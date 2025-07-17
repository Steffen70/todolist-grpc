<script setup>
import { RouterLink, RouterView } from "vue-router";
import Todo from "./components/Todo.vue";
</script>

<template>
    <v-app>
        <v-container class="my-container" fluid>
            <v-row>
                <v-col>
                    <header></header>
                    <h1>My Todo List App</h1>
                    <Todo />
                </v-col>
            </v-row>
        </v-container>
    </v-app>
</template>

<style scoped>
.my-container {
    max-width: 1000px !important;
    width: 80vw !important;
    margin-left: auto !important;
    margin-right: auto !important;
}
</style>
