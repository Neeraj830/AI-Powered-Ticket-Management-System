import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function CheckAuth({ children, protectedRoute }) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");

    const timer = setTimeout(() => {
      if (protectedRoute) {
        if (!token) {
          navigate("/login");
        } else {
          setLoading(false);
        }
      } else {
        if (token) {
          navigate("/");
        } else {
          setLoading(false);
        }
      }
    }, 100); // slight delay to simulate check

    return () => clearTimeout(timer);
  }, [navigate, protectedRoute]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Checking auth...</span>
        </div>
      </div>
    );
  }

  return children;
}
