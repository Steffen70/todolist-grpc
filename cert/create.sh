#!/bin/bash
set -e

# Clean up: delete all files except *.md and *.conf
find . -maxdepth 1 -type f ! \( -name "*.md" -o -name "*.conf" -o -name "create.sh" -o -name "create_localhost_cert.sh" -o -name ".gitignore" -o -name "api.crt" \) -exec rm -f {} +

echo "[1/6] Generating Root CA key (root_ca.key)..."
openssl genrsa -out root_ca.key 2048

echo "[2/6] Creating Root CA certificate (root_ca.crt)..."
openssl req -x509 -new -nodes -key root_ca.key -sha256 -days 1024 -out root_ca.crt -config root_ca.conf

echo "[3/6] Generating localhost private key (localhost.key)..."
openssl genrsa -out localhost.key 2048

echo "[4/6] Creating localhost CSR (localhost.csr)..."
openssl req -new -key localhost.key -out localhost.csr -config localhost.conf

echo "[5/6] Signing localhost certificate with Root CA (localhost.crt)..."
openssl x509 -req \
  -in localhost.csr \
  -CA root_ca.crt \
  -CAkey root_ca.key \
  -CAcreateserial \
  -out localhost.crt \
  -days 500 \
  -sha256 \
  -extfile localhost.conf \
  -extensions req_ext > /dev/null 2>&1

echo "[6/6] Creating passwordless PFX (localhost.pfx)..."
openssl pkcs12 -export \
  -out localhost.pfx \
  -inkey localhost.key \
  -in localhost.crt \
  -certfile root_ca.crt \
  -passout pass:

echo "Done"
echo ""
echo "You can now install the Root CA certificate (\"root_ca.crt\") as a trusted certificate authority on your system and browsers."