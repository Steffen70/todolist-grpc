from flask import Flask, render_template, request
import grpc
import os

# Import generated gRPC code
from generated import greet_pb2, greet_pb2_grpc

app = Flask(__name__)

CERT_PATH = os.path.abspath(os.path.join(os.path.dirname(__file__), "../cert/root_ca.crt"))

def create_greeter_stub():
    with open(CERT_PATH, "rb") as f:
        root_ca = f.read()
    creds = grpc.ssl_channel_credentials(root_ca)
    channel = grpc.secure_channel("localhost:8445", creds)
    return greet_pb2_grpc.GreeterStub(channel)

@app.route("/", methods=["GET", "POST"])
def index():
    greeting = None
    if request.method == "POST":
        name = request.form.get("name", "")
        stub = create_greeter_stub()
        request_msg = greet_pb2.HelloRequest(name=name)
        try:
            response = stub.SayHello(request_msg, timeout=5)
            greeting = response.message
        except grpc.RpcError as e:
            greeting = f"Error: {e.details()}"
    return render_template("index.html", greeting=greeting)
