import { useState } from "react";
import { useNavigate } from "react-router-dom";

export default function SignupPage() {
  const [form, setForm] = useState({
    email: "",
    password: "",
    role: "user",
    skills: [],
  });
  const [skillInput, setSkillInput] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSkillKeyDown = (e) => {
    if (e.key === "Enter") {
      e.preventDefault();
      const trimmed = skillInput.trim();
      if (trimmed && !form.skills.includes(trimmed)) {
        setForm({ ...form, skills: [...form.skills, trimmed] });
        setSkillInput("");
      }
    }
  };

  const removeSkill = (skillToRemove) => {
    setForm({
      ...form,
      skills: form.skills.filter((skill) => skill !== skillToRemove),
    });
  };

  const handleSignup = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await fetch(`https://localhost:7068/api/Auth/signup`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(form),
      });

      const data = await res.json();

      if (res.ok) {
        localStorage.setItem("token", data.token);
        localStorage.setItem("user", JSON.stringify(data.user));
        navigate("/");
      } else {
        alert(data.message || "Signup failed");
      }
    } catch (err) {
      alert("Something went wrong");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100 bg-body-secondary">
      <div className="card shadow-lg p-4 rounded-4" style={{ width: "100%", maxWidth: "480px" }}>
        <h3 className="text-center mb-4 text-primary fw-bold">Create Account</h3>

        <form onSubmit={handleSignup}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">Email address</label>
            <input
              type="email"
              className="form-control"
              id="email"
              name="email"
              value={form.email}
              onChange={handleChange}
              placeholder="you@example.com"
              required
            />
          </div>

          <div className="mb-3">
            <label htmlFor="password" className="form-label">Password</label>
            <div className="input-group">
              <input
                type={showPassword ? "text" : "password"}
                className="form-control"
                id="password"
                name="password"
                value={form.password}
                onChange={handleChange}
                placeholder="Enter password"
                required
              />
              <button
                type="button"
                className="btn btn-outline-secondary"
                onClick={() => setShowPassword(!showPassword)}
                tabIndex={-1}
              >
                {showPassword ? "🙈" : "👁️"}
              </button>
            </div>
          </div>

          <div className="mb-3">
            <label htmlFor="role" className="form-label">Role</label>
            <select
              className="form-select"
              id="role"
              name="role"
              value={form.role}
              onChange={handleChange}
            >
              <option value="user">User</option>
              <option value="admin">Admin</option>
              <option value="moderator">Moderator</option>
            </select>
          </div>

          <div className="mb-3">
            <label className="form-label">Skills</label>
            <input
              type="text"
              className="form-control"
              placeholder="Type a skill and press Enter"
              value={skillInput}
              onChange={(e) => setSkillInput(e.target.value)}
              onKeyDown={handleSkillKeyDown}
            />
            {form.skills.length > 0 && (
              <div className="mt-2 d-flex flex-wrap gap-2">
                {form.skills.map((skill, idx) => (
                  <span key={idx} className="badge text-bg-secondary d-flex align-items-center">
                    {skill}
                    <button
                      type="button"
                      className="btn-close btn-close-white btn-sm ms-2"
                      aria-label="Remove"
                      onClick={() => removeSkill(skill)}
                      style={{ fontSize: "0.6rem" }}
                    ></button>
                  </span>
                ))}
              </div>
            )}
          </div>

          <button type="submit" className="btn btn-primary w-100 fw-semibold" disabled={loading}>
            {loading ? (
              <>
                <span className="spinner-border spinner-border-sm me-2"></span>
                Signing up...
              </>
            ) : (
              "Sign Up"
            )}
          </button>
        </form>
      </div>
    </div>
  );
}
