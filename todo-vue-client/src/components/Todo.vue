<template>
    <div>
        <h2>Create New Todo List</h2>
        <input v-model="newListName" placeholder="List name" />
        <button @click="createList">Create</button>

        <hr />

        <div v-for="list in lists" :key="list.id" class="list">
            <h3>
                {{ list.listName }}
                <button @click="deleteList(list.id)">🗑️</button>
            </h3>

            <input v-model="newItems[list.id]" placeholder="Add new item" @keyup.enter="addItem(list.id)" />
            <button @click="addItem(list.id)">+</button>

            <ul>
                <li v-for="item in list.items" :key="item.id">
                    <input v-if="editingItemId === item.id" v-model="editedItemName" @keyup.enter="saveItemEdit(item, list.id)" @blur="cancelEdit" autofocus />
                    <span v-else @dblclick="startEditing(item)">
                        <input type="checkbox" :checked="item.isDone" :disabled="item.isDone" @change="toggleItemDone(list.id, item)" />
                        {{ item.itemName }}
                        <button @click="deleteItem(list.id, item.id)">🗑️</button>
                    </span>
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
import { TodoClient } from "grpc-stubs/todo_grpc_web_pb";
import { Empty } from "google-protobuf/google/protobuf/empty_pb";
import { CreateListTodoRequest, UpdateItemTodoRequest, DeleteListTodoRequest, DeleteItemTodoRequest, MarkAsDoneTodoRequest, AddItemTodoRequest } from "grpc-stubs/todo_pb";

export default {
    data() {
        return {
            lists: [],
            newListName: "",
            newItems: {},
            editingItemId: null,
            editedItemName: "",
        };
    },
    mounted() {
        this.fetchLists();
    },
    methods: {
        fetchLists() {
            const client = new TodoClient("https://localhost:8443");

            const request = new Empty();
            client.readLists(request, {}, (err, response) => {
                if (err) {
                    console.error("Failed to load lists:", err.message);
                    return;
                }

                const rawLists = response.getListsList();

                const parsedLists = rawLists.map(list => ({
                    id: list.getId(),
                    listName: list.getListname(),
                    items: list.getItemsList().map(item => ({
                        id: item.getId(),
                        itemName: item.getItemname(),
                        isDone: item.getIsdone(),
                    })),
                }));

                this.lists = parsedLists;
            });
        },
        createList() {
            if (!this.newListName.trim()) {
                alert("Please enter a list name.");
                return;
            }

            const client = new TodoClient("https://localhost:8443");

            const request = new CreateListTodoRequest();
            request.setListname(this.newListName);

            client.createList(request, {}, (err, response) => {
                if (err) {
                    console.error("Failed to create list:", err.message);
                    return;
                }

                console.log("Created list with ID:", response.getId());

                this.newListName = "";
                this.fetchLists();
            });
        },
        addItem(listId) {
            const itemName = this.newItems[listId];
            if (!itemName || !itemName.trim()) {
                alert("Please enter an item name.");
                return;
            }

            const client = new TodoClient("https://localhost:8443");
            const request = new AddItemTodoRequest();

            request.setTodolistid(listId);
            request.setItemname(itemName);

            client.addItemToList(request, {}, (err, response) => {
                if (err) {
                    console.error("Error when adding the item:", err.message);
                    return;
                }

                console.log("Added item:", response.getId());
                this.newItems[listId] = "";
                this.fetchLists();
            });
        },
        startEditing(item) {
            this.editingItemId = item.id;
            this.editedItemName = item.itemName;
        },

        cancelEdit() {
            this.editingItemId = null;
            this.editedItemName = "";
        },

        saveItemEdit(item, listId) {
            const client = new TodoClient("https://localhost:8443");
            const request = new UpdateItemTodoRequest();

            request.setTodolistid(listId);
            request.setId(item.id);
            request.setItemname(this.editedItemName);
            request.setIsdone(item.isDone);

            client.updateItem(request, {}, (err, response) => {
                if (err) {
                    console.error("Error when updating the item:", err.message);
                    return;
                }

                console.log("Updated item:", response.getId());
                this.cancelEdit();
                this.fetchLists();
            });
        },
        deleteList(listId) {
            if (!confirm("Do you really want to delete this list?")) {
                return;
            }

            const client = new TodoClient("https://localhost:8443");
            console.log(DeleteListTodoRequest, "DeleteListTodoRequest");
            const request = new DeleteListTodoRequest();

            request.setId(listId);

            client.deleteList(request, {}, (err, response) => {
                if (err) {
                    console.error("Error when deleting the list:", err.message);
                    return;
                }

                console.log("Deleted list:", response.getId());
                this.fetchLists();
            });
        },
        deleteItem(listId, itemId) {
            if (!confirm("Do you really want to delete this item?")) {
                return;
            }

            const client = new TodoClient("https://localhost:8443");
            const request = new DeleteItemTodoRequest();
            request.setTodolistid(listId);
            request.setId(itemId);

            client.deleteItem(request, {}, (err, response) => {
                if (err) {
                    console.error("Error when deleting the item:", err.message);
                    return;
                }

                console.log("Deleted item:", response.getId());
                this.fetchLists();
            });
        },
        toggleItemDone(listId, item) {
            const client = new TodoClient("https://localhost:8443");

            const request = new MarkAsDoneTodoRequest();
            request.setTodolistid(listId);
            request.setId(item.id);

            client.markAsDone(request, {}, (err, response) => {
                if (err) {
                    console.error("Error when setting the item done:", err.message);
                    return;
                }

                console.log("Changed item done state:", response.getIsdone());
                this.fetchLists();
            });
        },
    },
};
</script>
