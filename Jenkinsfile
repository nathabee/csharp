pipeline {
    agent any

    options {
        timestamps()
        disableConcurrentBuilds()
    }

    environment {
        IMAGE_NAME = 'dialmock'
        IMAGE_TAG  = "${env.BUILD_NUMBER}"
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore DialMock.slnx'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build DialMock.slnx -c Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh '''
                    mkdir -p TestResults
                    dotnet test DialMock.Tests/DialMock.Tests.csproj \
                      -c Release \
                      --no-build \
                      --logger "trx;LogFileName=DialMock.Tests.trx" \
                      --results-directory TestResults
                '''
            }
            post {
                always {
                    archiveArtifacts artifacts: 'TestResults/**/*', fingerprint: true
                }
            }
        }

        stage('Publish App') {
            steps {
                sh '''
                    rm -rf publish
                    dotnet publish DialMock/DialMock.csproj \
                      -c Release \
                      --no-build \
                      -o publish
                '''
            }
            post {
                always {
                    archiveArtifacts artifacts: 'publish/**/*', fingerprint: true
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                sh '''
                    docker build \
                      -t ${IMAGE_NAME}:${IMAGE_TAG} \
                      -t ${IMAGE_NAME}:latest \
                      .
                '''
            }
        }

        /*
        stage('Push Docker Image') {
            when {
                branch 'main'
            }
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'docker-registry-creds',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh '''
                        echo "$DOCKER_PASS" | docker login -u "$DOCKER_USER" --password-stdin
                        docker tag ${IMAGE_NAME}:${IMAGE_TAG} your-registry/${IMAGE_NAME}:${IMAGE_TAG}
                        docker tag ${IMAGE_NAME}:latest your-registry/${IMAGE_NAME}:latest
                        docker push your-registry/${IMAGE_NAME}:${IMAGE_TAG}
                        docker push your-registry/${IMAGE_NAME}:latest
                    '''
                }
            }
        }
        */
    }

    post {
        always {
            cleanWs()
        }
    }
}