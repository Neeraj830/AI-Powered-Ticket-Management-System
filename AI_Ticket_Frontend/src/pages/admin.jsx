// src/pages/AdminPanel.jsx
import React, { useEffect, useState } from "react";
import AlertMessage from "../components/AlertMessage";

export default function AdminPanel() {
  const [users, setUsers] = useState([]);
  const [filteredUsers, setFilteredUsers] = useState([]);
  const [editingUser, setEditingUser] = useState(null);
  const [formData, setFormData] = useState({ role: "", skills: "" });
  const [searchQuery, setSearchQuery] = useState("");
  const [alert, setAlert] = useState({ message: "", type: "" });

  const token = localStorage.getItem("token");

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const res = await fetch(`https://localhost:7068/api/Auth/all`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const data = await res.json();
      if (res.ok) {
        setUsers(data);
        setFilteredUsers(data);
      } else {
        showAlert(data.error || "Failed to fetch users", "danger");
      }
    } catch (err) {
      showAlert("Error fetching users", "danger");
    }
  };

  const showAlert = (message, type = "success") => {
    setAlert({ message, type });
    setTimeout(() => setAlert({ message: "", type: "" }), 3000);
  };

  const handleEditClick = (user) => {
    setEditingUser(user.email);
    setFormData({
      role: user.role,
      skills: user.skills?.join(", ") || "",
    });
  };

  const handleUpdate = async () => {
    try {
      const res = await fetch(`https://localhost:7068/api/Auth/update`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          email: editingUser,
          role: formData.role,
          skills: formData.skills
            .split(",")
            .map((s) => s.trim())
            .filter(Boolean),
        }),
      });

      const data = await res.json();
      if (!res.ok) {
        showAlert(data.error || "Update failed", "danger");
        return;
      }

      showAlert("User updated successfully!", "success");
      setEditingUser(null);
      setFormData({ role: "", skills: "" });
      fetchUsers();
    } catch (err) {
      showAlert("Update failed", "danger");
    }
  };

  const handleSearch = (e) => {
    const query = e.target.value.toLowerCase();
    setSearchQuery(query);
    setFilteredUsers(
      users.filter((user) => user.email.toLowerCase().includes(query))
    );
  };

  return (
    <div className="container mt-5">
      <h2 className="text-center mb-4">👨‍💼 Admin Panel - Manage Users</h2>

      <div className="input-group mb-4 shadow-sm">
        <span className="input-group-text bg-white border-end-0">
          <i className="bi bi-search"></i>
        </span>
        <input
          type="text"
          className="form-control border-start-0"
          placeholder="Search by email"
          value={searchQuery}
          onChange={handleSearch}
        />
      </div>

      <div className="row g-4">
        {filteredUsers.map((user) => (
          <div className="col-md-6 col-lg-4" key={user._id}>
            <div className="card shadow-sm border-0 h-100 d-flex flex-column">
              <div className="card-body d-flex flex-column">
                <div className="mb-3">
                  <p className="mb-1">
                    <strong>Email:</strong> {user.email}
                  </p>
                  <p className="mb-1">
                    <strong>Role:</strong> {user.role}
                  </p>
                  <p className="mb-0">
                    <strong>Skills:</strong>{" "}
                    {user.skills?.length > 0 ? user.skills.join(", ") : "N/A"}
                  </p>
                </div>

                {editingUser === user.email ? (
                  <div className="mt-auto">
                    <div className="mb-2">
                      <select
                        className="form-select"
                        value={formData.role}
                        onChange={(e) =>
                          setFormData({ ...formData, role: e.target.value })
                        }
                      >
                        <option value="user">User</option>
                        <option value="moderator">Moderator</option>
                        <option value="admin">Admin</option>
                      </select>
                    </div>

                    <div className="mb-2">
                      <input
                        type="text"
                        className="form-control"
                        placeholder="Comma-separated skills"
                        value={formData.skills}
                        onChange={(e) =>
                          setFormData({ ...formData, skills: e.target.value })
                        }
                      />
                    </div>

                    <div className="d-flex gap-2">
                      <button
                        className="btn btn-success btn-sm w-100"
                        onClick={handleUpdate}
                      >
                        ✅ Save
                      </button>
                      <button
                        className="btn btn-outline-secondary btn-sm w-100"
                        onClick={() => setEditingUser(null)}
                      >
                        ❌ Cancel
                      </button>
                    </div>
                  </div>
                ) : (
                  <div className="mt-auto">
                    <button
                      className="btn btn-primary btn-sm w-100"
                      onClick={() => handleEditClick(user)}
                    >
                      ✏️ Edit
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* ✅ Alert Message */}
      <AlertMessage
        message={alert.message}
        type={alert.type}
        onClose={() => setAlert({ message: "", type: "" })}
      />
    </div>
  );
}
