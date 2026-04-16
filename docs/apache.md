# Target architecture for DialMock

Your runtime container will listen:

```
127.0.0.1:18080 -> container:8080
```

Apache will expose:

```
https://dialmock.nathabee.de/
           ↓
http://127.0.0.1:18080/
```

So the full chain becomes:

```
Internet
   ↓
dialmock.nathabee.de
   ↓
Apache :443
   ↓
127.0.0.1:18080
   ↓
Docker container (DialMock)
```

 
---

# Step 1 — DNS in Hetzner

Same as before.

Add:

### IPv4

```
A dialmock -> VPS_IPV4
```

### IPv6

```
AAAA dialmock -> VPS_IPV6
```

Wait until:

```bash
ping dialmock.nathabee.de
```

returns your server.

---

# Step 2 — verify container locally

Before touching Apache:

```bash
curl -I http://127.0.0.1:18080/
```

If this fails, Apache will also fail.

So only continue if this works.

---

# Step 3 — HTTP vhost (ACME + redirect)

File:

```
/etc/apache2/sites-available/dialmock-http.conf
```

```bash
sudo tee /etc/apache2/sites-available/dialmock-http.conf >/dev/null <<'APACHE'
<VirtualHost *:80>
    ServerName dialmock.nathabee.de

    Alias /.well-known/acme-challenge/ /var/www/certbot/.well-known/acme-challenge/
    <Directory "/var/www/certbot/.well-known/acme-challenge/">
        Options None
        AllowOverride None
        Require all granted
    </Directory>

    RewriteEngine On
    RewriteCond %{REQUEST_URI} !^/\.well-known/acme-challenge/
    RewriteRule ^ https://%{HTTP_HOST}%{REQUEST_URI} [R=301,L]

    ErrorLog  ${APACHE_LOG_DIR}/dialmock-http-error.log
    CustomLog ${APACHE_LOG_DIR}/dialmock-http-access.log combined
</VirtualHost>
APACHE
```

Enable:

```bash
sudo a2ensite dialmock-http.conf
sudo apache2ctl configtest
sudo systemctl reload apache2
```

---

# Step 4 — Request certificate

```bash
sudo certbot certonly --webroot \
  -w /var/www/certbot \
  -d dialmock.nathabee.de \
  -m admin@nathabee.de \
  --agree-tos \
  --no-eff-email
```

---

# Step 5 — HTTPS reverse proxy

File:

```
/etc/apache2/sites-available/dialmock-ssl.conf
```

```bash
sudo tee /etc/apache2/sites-available/dialmock-ssl.conf >/dev/null <<'APACHE'
<VirtualHost *:443>
    ServerName dialmock.nathabee.de

    SSLEngine on
    SSLCertificateFile /etc/letsencrypt/live/dialmock.nathabee.de/fullchain.pem
    SSLCertificateKeyFile /etc/letsencrypt/live/dialmock.nathabee.de/privkey.pem
    Include /etc/letsencrypt/options-ssl-apache.conf

    ProxyPreserveHost On
    ProxyRequests Off

    RequestHeader set X-Forwarded-Proto "https"
    RequestHeader set X-Forwarded-Port "443"

    Header always set X-Content-Type-Options "nosniff"
    Header always set X-Frame-Options "SAMEORIGIN"

    ProxyPass        / http://127.0.0.1:18080/ connectiontimeout=5 timeout=60
    ProxyPassReverse / http://127.0.0.1:18080/

    <Proxy http://127.0.0.1:18080/*>
        Require all granted
    </Proxy>

    ErrorLog  ${APACHE_LOG_DIR}/dialmock-ssl-error.log
    CustomLog ${APACHE_LOG_DIR}/dialmock-ssl-access.log combined
</VirtualHost>
APACHE
```

Enable:

```bash
sudo a2ensite dialmock-ssl.conf
sudo apache2ctl configtest
sudo systemctl reload apache2
```

---

# Step 6 — Reload hook (reuse if exists)

You already created:

```
/etc/letsencrypt/renewal-hooks/deploy/reload-apache.sh
```

So nothing new needed.

---

# Step 7 — Test locally

```bash
curl -I http://dialmock.nathabee.de/
curl -I https://dialmock.nathabee.de/
```

Then in browser:

```
https://dialmock.nathabee.de
```

---
