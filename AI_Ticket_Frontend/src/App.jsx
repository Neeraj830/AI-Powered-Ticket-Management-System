// App.jsx
import { Outlet } from "react-router-dom";
import Navbar from "./components/navbar";

export default function App() {
  return (
    <div>
      <Navbar />
      <main className="p-4">
        <Outlet />
      </main>
    </div>
  );
}
