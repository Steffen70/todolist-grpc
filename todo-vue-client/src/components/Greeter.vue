<template>
    <div>
        <input v-model="name" placeholder="Enter your name" />
        <button @click="sayHello">Say Hello</button>
        <p v-if="greeting">{{ greeting }}</p>
    </div>
</template>

<script>
import * as pb from "grpc-stubs/greet_pb.js";
import * as grpcWeb from "grpc-stubs/greet_grpc_web_pb.js";

export default {
    data() {
        return {
            name: "",
            greeting: "",
        };
    },
    methods: {
        sayHello() {
            const client = new pb.GreeterClient("https://localhost:8443");
            const request = new grpcWeb.HelloRequest();
            request.setName(this.name);
            client.sayHello(request, {}, (err, response) => {
                if (err) {
                    this.greeting = "Error: " + err.message;
                } else {
                    this.greeting = response.getMessage();
                }
            });
        },
    },
};
</script>
