import React, { useEffect, useState, useCallback } from "react";
import * as pb from "grpc-stubs/todo_pb.js";
import * as grpcWeb from "grpc-stubs/todo_grpc_web_pb.js";
import { Empty } from "google-protobuf/google/protobuf/empty_pb.js";

const client = new grpcWeb.TodoClient("https://localhost:8443");

export const TodoList = () => {
    const [lists, setLists] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [newItemTexts, setNewItemTexts] = useState({});
    const [newListName, setNewListName] = useState("");
    const [editingItemId, setEditingItemId] = useState(null);
    const [editText, setEditText] = useState("");

    const fetchLists = useCallback(() => {
        setLoading(true);
        const request = new Empty(); // since the ReadLists function doesn't need any input parameters.. I read I'm supposed to send an "Empty" message for this purpose
        client.readLists(request, {}, (err, response) => {
            if (err) {
                console.error("Error fetching lists:", err.message);
                setError(err.message);
                setLoading(false);
                return;
            }
            const fetchedLists = response.getListsList().map(list => list.toObject()); // iterate through the array to create an easy to read (and to work with in react) object.. I was hinted here.
            setLists(fetchedLists);
            console.log(fetchedLists);
            setLoading(false);
        });
    }, []);

    useEffect(() => {
        fetchLists();
    }, [fetchLists]);

    const handleCreateList = () => {
        if (!newListName.trim()) return;

        const request = new pb.CreateListTodoRequest();
        request.setListName(newListName);

        client.createList(request, {}, (err, response) => {
            if (err) {
                console.error("Error creating list:", err.message);
                setError(err.message);
                return;
            }
            setNewListName("");
            fetchLists();
        });
    };

    const handleAddItem = listId => {
        const text = newItemTexts[listId] || "";
        if (!text.trim()) return;

        const request = new pb.AddItemTodoRequest();
        request.setTodoListId(listId);
        request.setItemName(text);

        client.addItemToList(request, {}, (err, response) => {
            if (err) {
                console.error("Error adding item:", err.message);
                setError(err.message);
                return;
            }
            setNewItemTexts(prev => ({ ...prev, [listId]: "" }));
            fetchLists();
        });
    };

    const handleDeleteItem = (listId, itemId) => {
        if (!window.confirm("Are you sure you want to delete this item?")) return;

        const request = new pb.DeleteItemTodoRequest();
        request.setTodoListId(listId);
        request.setId(itemId);

        client.deleteItem(request, {}, (err, response) => {
            if (err) {
                console.error("Error deleting item:", err.message);
                setError(err.message);
                return;
            }
            fetchLists();
        });
    };

    const handleToggleDone = (listId, item) => {
        const request = new pb.UpdateItemTodoRequest();

        request.setTodoListId(listId); // had to pass this as well, as specified in the proto file
        request.setId(item.id);
        request.setItemName(item.itemName);
        request.setIsDone(!item.isDone); // to enable the two-way toggling..

        client.updateItem(request, {}, (err, response) => {
            if (err) {
                console.error("Error updating item:", err.message);
                setError(err.message);
                return;
            }
            fetchLists();
        });
    };

    const handleItemTextChange = (listId, text) => {
        setNewItemTexts(prev => ({
            ...prev,
            [listId]: text,
        }));
    };

    const handleSaveUpdate = (listId, item) => {
        if (!editText.trim()) return;
        const request = new pb.UpdateItemTodoRequest();
        request.setTodoListId(listId);
        request.setId(item.id);
        request.setItemName(editText);
        request.setIsDone(item.isDone);
        client.updateItem(request, {}, (err, response) => {
            if (err) {
                setError(err.message);
                return;
            }
            setEditingItemId(null);
            setEditText("");
            fetchLists();
        });
    };

    const handleStartEdit = item => {
        setEditingItemId(item.id);
        setEditText(item.itemName);
    };

    const handleCancelEdit = () => {
        setEditingItemId(null);
        setEditText("");
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: "red" }}>Error: {error}</div>;

    return (
        <div style={{ fontFamily: "sans-serif", maxWidth: "1200px", margin: "auto" }}>
            <h1>Todo Lists</h1>
            <div style={{ marginBottom: "2rem", padding: "1rem", border: "1px solid #ddd", borderRadius: "8px" }}>
                <h2>Create a New List</h2>
                <input type="text" value={newListName} onChange={e => setNewListName(e.target.value)} placeholder="New list name..." />
                <button onClick={handleCreateList}>Create List</button>
            </div>
            <div style={{ display: "flex", flexWrap: "wrap", gap: "1rem" }}>
                {lists.map(list => (
                    <div key={list.id} style={{ border: "1px solid #ccc", padding: "1rem", borderRadius: "8px", flex: "1 1 300px" }}>
                        <h2>{list.listName}</h2>
                        <div style={{ marginBottom: "1rem" }}>
                            <input type="text" value={newItemTexts[list.id] || ""} onChange={e => handleItemTextChange(list.id, e.target.value)} placeholder="New todo item..." />
                            <button onClick={() => handleAddItem(list.id)}>Add Item</button>
                        </div>
                        <ul>
                            {list.itemsList?.map(item => (
                                <li key={item.id} style={{ display: "flex", alignItems: "center", justifyContent: "space-between", padding: "0.5rem 0" }}>
                                    {editingItemId === item.id ? (
                                        <div style={{ display: "flex", alignItems: "center", width: "100%" }}>
                                            <input type="text" value={editText} onChange={e => setEditText(e.target.value)} style={{ flexGrow: 1 }} />
                                            <button onClick={() => handleSaveUpdate(list.id, item)} style={{ marginLeft: "5px", color: "green" }}>
                                                Save
                                            </button>
                                            <button onClick={handleCancelEdit} style={{ marginLeft: "5px", color: "gray" }}>
                                                Cancel
                                            </button>
                                        </div>
                                    ) : (
                                        <>
                                            <div style={{ display: "flex", alignItems: "center" }}>
                                                <input type="checkbox" checked={item.isDone} onChange={() => handleToggleDone(list.id, item)} style={{ marginRight: "10px" }} />
                                                <span style={{ textDecoration: item.isDone ? "line-through" : "none" }}>{item.itemName}</span>
                                            </div>
                                            <div>
                                                <button onClick={() => handleStartEdit(item)} style={{ color: "blue", marginRight: "5px" }}>
                                                    Edit
                                                </button>
                                                <button onClick={() => handleDeleteItem(list.id, item.id)} style={{ color: "red" }}>
                                                    Delete
                                                </button>
                                            </div>
                                        </>
                                    )}
                                </li>
                            ))}
                        </ul>
                    </div>
                ))}
            </div>
        </div>
    );
};
