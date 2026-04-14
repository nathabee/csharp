using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(WikaDialPlugin.DialCommands))]

namespace WikaDialPlugin
{
    public class DialCommands : IExtensionApplication
    {
        private readonly IDialDataProvider _dataProvider = new MockDialDataProvider();
        private readonly DialRuleEngine _ruleEngine = new DialRuleEngine();
        private readonly DialDrawer _drawer = new DialDrawer();
        private readonly DialValidator _validator = new DialValidator();
        private readonly ExportService _exportService = new ExportService();

        public void Initialize()
        {
            // Assembly loaded into AutoCAD.
        }

        public void Terminate()
        {
            // AutoCAD shutdown / unload.
        }

        [CommandMethod("WIKA_GENERATE_DIAL")]
        public void GenerateDial()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            string productCode = PromptString(ed, "\nProduct code");
            if (string.IsNullOrWhiteSpace(productCode))
            {
                ed.WriteMessage("\nNo product code entered.");
                return;
            }

            DialSpec spec;
            try
            {
                spec = _dataProvider.LoadByProductCode(productCode);
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\nData load failed: {ex.Message}");
                return;
            }

            ValidationResult specValidation = _ruleEngine.ValidateSpec(spec);
            if (!specValidation.IsValid)
            {
                WriteErrors(ed, "Specification validation failed", specValidation.Errors);
                return;
            }

            DialGeometry geometry = _ruleEngine.BuildGeometry(spec);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    _drawer.Draw(db, tr, spec, geometry);
                    tr.Commit();
                    ed.WriteMessage("\nDial generated successfully.");
                }
                catch (Exception ex)
                {
                    ed.WriteMessage($"\nDial generation failed: {ex.Message}");
                }
            }
        }

        [CommandMethod("WIKA_VALIDATE_DIAL")]
        public void ValidateDial()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            string productCode = PromptString(ed, "\nProduct code for validation");
            if (string.IsNullOrWhiteSpace(productCode))
            {
                ed.WriteMessage("\nNo product code entered.");
                return;
            }

            DialSpec spec;
            try
            {
                spec = _dataProvider.LoadByProductCode(productCode);
            }
            catch (Exception ex)
            {
                ed.WriteMessage($"\nData load failed: {ex.Message}");
                return;
            }

            ValidationResult result = _validator.ValidateDrawing(db, spec);

            if (result.IsValid)
            {
                ed.WriteMessage("\nDrawing validation passed.");
            }
            else
            {
                WriteErrors(ed, "Drawing validation failed", result.Errors);
            }
        }

        [CommandMethod("WIKA_BATCH_UPDATE")]
        public void BatchUpdate()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            ed.WriteMessage(
                "\nBatch update skeleton: load product list, fetch each DialSpec, regenerate or update layouts, validate, then export."
            );

            // Realistic batch flow:
            // 1. Read changed products from SQL / ERP / CSV.
            // 2. For each product:
            //    - Load spec
            //    - Validate spec
            //    - Open or create drawing
            //    - Update dial content
            //    - Validate drawing
            //    - Export PDF / save DWG
            // 3. Write report
        }

        private static string PromptString(Editor ed, string message)
        {
            PromptResult result = ed.GetString($"\n{message}: ");
            return result.Status == PromptStatus.OK ? result.StringResult.Trim() : string.Empty;
        }

        private static void WriteErrors(Editor ed, string title, IEnumerable<string> errors)
        {
            ed.WriteMessage($"\n{title}:");
            foreach (string error in errors)
            {
                ed.WriteMessage($"\n - {error}");
            }
        }
    }

    public interface IDialDataProvider
    {
        DialSpec LoadByProductCode(string productCode);
    }

    public class MockDialDataProvider : IDialDataProvider
    {
        public DialSpec LoadByProductCode(string productCode)
        {
            // Replace this with SQL / CSV / REST / ERP connector.
            return new DialSpec
            {
                ProductCode = productCode,
                Customer = "ABI",
                Title = "HYDRAULIC PRESSURE",
                UnitPrimary = "bar",
                UnitSecondary = "psi",
                MinValue = 0,
                MaxValue = 600,
                WarningStart = 480,
                MajorStep = 100,
                MinorDivisionsPerMajor = 4,
                TemplateName = "GaugeDial_A_52mm",
                LogoBlockName = "ABI_LOGO",
                OutputPath = @"C:\temp\dial-output"
            };
        }
    }

    public class DialRuleEngine
    {
        public ValidationResult ValidateSpec(DialSpec spec)
        {
            List<string> errors = new();

            if (string.IsNullOrWhiteSpace(spec.ProductCode))
                errors.Add("Product code is missing.");

            if (string.IsNullOrWhiteSpace(spec.Customer))
                errors.Add("Customer is missing.");

            if (spec.MaxValue <= spec.MinValue)
                errors.Add("MaxValue must be greater than MinValue.");

            if (spec.WarningStart <= spec.MinValue || spec.WarningStart >= spec.MaxValue)
                errors.Add("WarningStart must lie inside the measurement range.");

            if (spec.MajorStep <= 0)
                errors.Add("MajorStep must be positive.");

            if (spec.MinorDivisionsPerMajor <= 0)
                errors.Add("MinorDivisionsPerMajor must be positive.");

            return ValidationResult.FromErrors(errors);
        }

        public DialGeometry BuildGeometry(DialSpec spec)
        {
            // In real life: choose geometry from template family / gauge size.
            return new DialGeometry
            {
                Center = new Point3d(0, 0, 0),
                StartAngleDeg = 210,
                EndAngleDeg = 330,
                OuterRadius = 50,
                ScaleRadius = 43,
                MajorTickInnerRadius = 36,
                MinorTickInnerRadius = 39,
                LabelRadius = 30
            };
        }
    }

    public class DialDrawer
    {
        public void Draw(Database db, Transaction tr, DialSpec spec, DialGeometry geometry)
        {
            BlockTable bt =
                (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

            BlockTableRecord modelSpace =
                (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            DrawOutline(modelSpace, tr, geometry);
            DrawScaleArc(modelSpace, tr, geometry);
            DrawTicks(modelSpace, tr, spec, geometry);
            DrawLabels(modelSpace, tr, spec, geometry);
            DrawTitle(modelSpace, tr, spec, geometry);
            DrawWarningZone(modelSpace, tr, spec, geometry);

            // Realistic extension points:
            // DrawLogoBlock(...)
            // FillAttributes(...)
            // AddSecondaryScale(...)
            // AddPrintFrame(...)
        }

        private void DrawOutline(BlockTableRecord btr, Transaction tr, DialGeometry g)
        {
            AppendEntity(btr, tr, new Circle(g.Center, Vector3d.ZAxis, g.OuterRadius));
        }

        private void DrawScaleArc(BlockTableRecord btr, Transaction tr, DialGeometry g)
        {
            AppendEntity(
                btr,
                tr,
                new Arc(g.Center, g.ScaleRadius, DegToRad(g.StartAngleDeg), DegToRad(g.EndAngleDeg))
            );
        }

        private void DrawTicks(BlockTableRecord btr, Transaction tr, DialSpec spec, DialGeometry g)
        {
            int majorCount = (int)((spec.MaxValue - spec.MinValue) / spec.MajorStep);
            int totalMinorSteps = majorCount * spec.MinorDivisionsPerMajor;

            for (int i = 0; i <= totalMinorSteps; i++)
            {
                double t = totalMinorSteps == 0 ? 0 : (double)i / totalMinorSteps;
                double angleDeg = Lerp(g.StartAngleDeg, g.EndAngleDeg, t);
                bool isMajor = (i % spec.MinorDivisionsPerMajor) == 0;

                Point3d pOuter = Polar(g.Center, angleDeg, g.ScaleRadius);
                Point3d pInner = Polar(
                    g.Center,
                    angleDeg,
                    isMajor ? g.MajorTickInnerRadius : g.MinorTickInnerRadius
                );

                AppendEntity(btr, tr, new Line(pInner, pOuter));
            }
        }

        private void DrawLabels(BlockTableRecord btr, Transaction tr, DialSpec spec, DialGeometry g)
        {
            int majorCount = (int)((spec.MaxValue - spec.MinValue) / spec.MajorStep);

            for (int step = 0; step <= majorCount; step++)
            {
                double t = majorCount == 0 ? 0 : (double)step / majorCount;
                double angleDeg = Lerp(g.StartAngleDeg, g.EndAngleDeg, t);
                double value = spec.MinValue + (step * spec.MajorStep);

                Point3d pos = Polar(g.Center, angleDeg, g.LabelRadius);

                MText label = new MText
                {
                    Location = pos,
                    Contents = value.ToString("0"),
                    TextHeight = 2.5,
                    Attachment = AttachmentPoint.MiddleCenter
                };

                AppendEntity(btr, tr, label);
            }
        }

        private void DrawTitle(BlockTableRecord btr, Transaction tr, DialSpec spec, DialGeometry g)
        {
            MText title = new MText
            {
                Location = new Point3d(g.Center.X, g.Center.Y + 14, 0),
                Contents = $"{spec.Customer}\\P{spec.Title}",
                TextHeight = 3.0,
                Attachment = AttachmentPoint.MiddleCenter
            };

            AppendEntity(btr, tr, title);
        }

        private void DrawWarningZone(BlockTableRecord btr, Transaction tr, DialSpec spec, DialGeometry g)
        {
            double warningT = (spec.WarningStart - spec.MinValue) / (spec.MaxValue - spec.MinValue);
            double warningStartDeg = Lerp(g.StartAngleDeg, g.EndAngleDeg, warningT);

            Arc warningArc = new Arc(
                g.Center,
                g.ScaleRadius - 2.0,
                DegToRad(warningStartDeg),
                DegToRad(g.EndAngleDeg)
            );

            AppendEntity(btr, tr, warningArc);
        }

        private static void AppendEntity(BlockTableRecord btr, Transaction tr, Entity entity)
        {
            btr.AppendEntity(entity);
            tr.AddNewlyCreatedDBObject(entity, true);
        }

        private static Point3d Polar(Point3d basePoint, double angleDeg, double distance)
        {
            double rad = DegToRad(angleDeg);
            return new Point3d(
                basePoint.X + Math.Cos(rad) * distance,
                basePoint.Y + Math.Sin(rad) * distance,
                basePoint.Z
            );
        }

        private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;

        private static double Lerp(double a, double b, double t) => a + ((b - a) * t);
    }

    public class DialValidator
    {
        public ValidationResult ValidateDrawing(Database db, DialSpec spec)
        {
            List<string> errors = new();

            // Skeleton only. Real validations could include:
            // - required title present
            // - logo block exists
            // - text inside printable area
            // - no overlap between labels and ticks
            // - correct layer usage
            // - correct template family

            if (string.IsNullOrWhiteSpace(spec.TemplateName))
                errors.Add("TemplateName is missing.");

            return ValidationResult.FromErrors(errors);
        }
    }

    public class ExportService
    {
        public void ExportPdf(string outputPath)
        {
            // Skeleton only.
            // Real version: configure plot settings and export PDF.
        }

        public void SaveDwg(string outputPath)
        {
            // Skeleton only.
            // Real version: save to controlled archive/output path.
        }
    }

    public class DialSpec
    {
        public string ProductCode { get; set; } = "";
        public string Customer { get; set; } = "";
        public string Title { get; set; } = "";
        public string UnitPrimary { get; set; } = "";
        public string UnitSecondary { get; set; } = "";
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double WarningStart { get; set; }
        public int MajorStep { get; set; }
        public int MinorDivisionsPerMajor { get; set; }
        public string TemplateName { get; set; } = "";
        public string LogoBlockName { get; set; } = "";
        public string OutputPath { get; set; } = "";
    }

    public class DialGeometry
    {
        public Point3d Center { get; set; }
        public double StartAngleDeg { get; set; }
        public double EndAngleDeg { get; set; }
        public double OuterRadius { get; set; }
        public double ScaleRadius { get; set; }
        public double MajorTickInnerRadius { get; set; }
        public double MinorTickInnerRadius { get; set; }
        public double LabelRadius { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new();

        public static ValidationResult FromErrors(IEnumerable<string> errors)
        {
            ValidationResult result = new ValidationResult();
            result.Errors.AddRange(errors);
            return result;
        }
    }
}