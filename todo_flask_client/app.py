from flask import Flask, render_template, request
import grpc
import os
from google.protobuf import empty_pb2

# Import generated gRPC code for todo service
from generated import todo_pb2, todo_pb2_grpc

app = Flask(__name__)

CERT_PATH = os.path.abspath(os.path.join(os.path.dirname(__file__), "../cert/root_ca.crt"))

def create_todo_stub():
    """Create a gRPC stub for the Todo service"""
    with open(CERT_PATH, "rb") as f:
        root_ca = f.read()
    creds = grpc.ssl_channel_credentials(root_ca)
    channel = grpc.secure_channel("localhost:8445", creds)
    return todo_pb2_grpc.TodoStub(channel)

@app.route("/", methods=["GET", "POST"])
def index():
    """Home page displaying all todo lists and their items"""
    lists = []
    error_message = None
    
    try:
        stub = create_todo_stub()
        # Call ReadLists with Empty message
        response = stub.ReadLists(empty_pb2.Empty(), timeout=5)
        lists = response.lists
    except grpc.RpcError as e:
        error_message = f"Error fetching todo lists: {e.details()}"
    except Exception as e:
        error_message = f"Unexpected error: {str(e)}"
    
    return render_template("index.html", lists=lists, error_message=error_message)

@app.route("/about")
def about():
    """About page"""
    return render_template("about.html")

if __name__ == "__main__":
    app.run(debug=True)
