from flask import Flask, render_template, request, flash
import grpc
import os

# Import generated gRPC code
from generated import todo_pb2, todo_pb2_grpc

app = Flask(__name__)

CERT_PATH = os.path.abspath(os.path.join(os.path.dirname(__file__), "../cert/root_ca.crt"))

def create_todo_stub():
    with open(CERT_PATH, "rb") as f:
        root_ca = f.read()
    creds = grpc.ssl_channel_credentials(root_ca)
    channel = grpc.secure_channel("localhost:8445", creds)
    return todo_pb2_grpc.TodoServiceStub(channel)

@app.route("/todo", methods=["GET", "POST"])
def todo():
    stub = create_todo_stub()  # TLS-Stub, wie du es schon vorbereitet hast
    todos = []

    if request.method == "POST":
        list_name = request.form.get("listName", "")
        try:
            create_resp = stub.CreateList(
                todo_pb2.CreateListTodoRequest(listName=list_name), timeout=5
            )
            flash(f"Liste '{list_name}' angelegt (ID: {create_resp.Id})", "success")
        except grpc.RpcError as e:
            flash(f"Fehler beim Erstellen der Liste: {e.details()}", "danger")

    try:
        read_resp = stub.ReadLists(todo_pb2.Empty(), timeout=5)
        todos = read_resp.Lists  # Liste von ListTodoItem-Objekten
    except grpc.RpcError as e:
        flash(f"Fehler beim Abrufen der Listen: {e.details()}", "danger")

    return render_template("todo.html", todos=todos)
