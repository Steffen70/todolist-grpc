import { useState, useEffect } from "react";
import reactLogo from "./assets/react.svg";
import viteLogo from "/vite.svg";
import "./App.css";
import { TodoList } from "./components/TodoList";

function App() {
    const [count, setCount] = useState(0);

    return <TodoList />;
}

export default App;
