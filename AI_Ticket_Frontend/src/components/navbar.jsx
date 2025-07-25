import { Link, useNavigate, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";

export default function Navbar() {
  const navigate = useNavigate();
  const location = useLocation();
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userData = localStorage.getItem("user");
    if (token && userData) {
      try {
        setUser(JSON.parse(userData));
      } catch {
        localStorage.removeItem("user");
      }
    }
  }, [location.pathname]);

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
    navigate("/login");
  };

  return (
    <nav className="navbar bg-base-200 shadow-md px-4 py-2">
      <div className="flex-1">
        <Link to="/" className="btn btn-ghost text-xl">
          Ticket AI
        </Link>
        {user && (
          <ul className="menu menu-horizontal px-1 hidden sm:flex">
            <li>
              <Link to="/" className={location.pathname === "/" ? "active" : ""}>Home</Link>
            </li>
            <li>
              <Link to="/tickets" className={location.pathname === "/tickets" ? "active" : ""}>Tickets</Link>
            </li>
            {user.role === "admin" && (
              <li>
                <Link to="/admin" className={location.pathname === "/admin" ? "active" : ""}>Admin</Link>
              </li>
            )}
          </ul>
        )}
      </div>

      <div className="flex items-center gap-3">
        {!user ? (
          <>
            <Link to="/signup" className="btn btn-sm btn-outline">
              Sign Up
            </Link>
            <Link to="/login" className="btn btn-sm btn-primary">
              Login
            </Link>
          </>
        ) : (
          <>
            <span className="text-sm text-gray-700 hidden sm:inline">
              Hi, {user.email}
            </span>
            <button onClick={logout} className="btn btn-sm btn-error">
              Logout
            </button>
          </>
        )}
      </div>
    </nav>
  );
}
