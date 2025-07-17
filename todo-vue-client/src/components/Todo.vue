<template>
    <v-container>
        <v-row>
            <!-- <v-col cols="12" md="6" class="mx-auto"> -->
            <v-col style="width: 100%">
                <v-card class="pa-4">
                    <v-card-title>Create New Todo List</v-card-title>

                    <v-text-field v-model="newListName" label="List name" dense clearable @keyup.enter="createList" />
                    <v-btn color="primary" @click="createList">Create</v-btn>
                </v-card>

                <v-divider class="my-6"></v-divider>

                <div v-for="list in lists" :key="list.id" class="mb-6">
                    <v-card>
                        <v-card-title class="d-flex justify-space-between align-center">
                            <div>{{ list.listName }}</div>
                            <v-btn icon color="red" @click="deleteList(list.id)" title="Delete list">
                                <v-icon>mdi-delete</v-icon>
                            </v-btn>
                        </v-card-title>

                        <v-card-text>
                            <v-row class="align-center mb-4">
                                <v-col>
                                    <v-text-field v-model="newItems[list.id]" label="Add new item" dense clearable @keyup.enter="addItem(list.id)" />
                                </v-col>
                                <v-col cols="auto">
                                    <v-btn color="primary" @click="addItem(list.id)">+</v-btn>
                                </v-col>
                            </v-row>

                            <v-list dense>
                                <v-list-item v-for="item in list.items" :key="item.id" class="d-flex align-center">
                                    <div v-if="editingItemId === item.id" class="flex-grow-1">
                                        <v-text-field v-model="editedItemName" dense autofocus @keyup.enter="saveItemEdit(item, list.id)" @blur="cancelEdit" />
                                    </div>

                                    <div v-else class="flex-grow-1 d-flex align-center" @dblclick="startEditing(item)" style="cursor: pointer">
                                        <v-checkbox v-model="item.isDone" :disabled="item.isDone" @change="toggleItemDone(list.id, item)" class="mr-3" hide-details />
                                        <span :style="{ textDecoration: item.isDone ? 'line-through' : 'none' }">
                                            {{ item.itemName }}
                                        </span>
                                    </div>

                                    <v-btn icon color="red" @click="deleteItem(list.id, item.id)" title="Delete item">
                                        <v-icon>mdi-delete</v-icon>
                                    </v-btn>
                                </v-list-item>
                            </v-list>
                        </v-card-text>
                    </v-card>
                </div>
            </v-col>
        </v-row>
    </v-container>
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
