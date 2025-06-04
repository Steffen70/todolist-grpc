#!/bin/bash
set -e

echo "[0/4] Cleaning up existing localhost certificate files..."
rm -f localhost.crt localhost.key localhost.csr localhost.pfx root_ca.srl

echo "[1/4] Generating localhost private key (localhost.key)..."
openssl genrsa -out localhost.key 2048

echo "[2/4] Creating localhost CSR (localhost.csr)..."
openssl req -new -key localhost.key -out localhost.csr -config localhost.conf

echo "[3/4] Signing localhost certificate with Root CA (localhost.crt)..."
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

echo "[4/4] Creating passwordless PFX (localhost.pfx)..."
openssl pkcs12 -export \
  -out localhost.pfx \
  -inkey localhost.key \
  -in localhost.crt \
  -certfile root_ca.crt \
  -passout pass:

echo ""
echo "Done. localhost.crt, localhost.key, and localhost.pfx were recreated and signed using root_ca.key."
