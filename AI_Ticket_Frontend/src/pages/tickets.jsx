import { useEffect, useState } from "react";

export default function Tickets() {
  const [form, setForm] = useState({ title: "", description: "" });
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(false);

  const token = localStorage.getItem("token");

  const fetchTickets = async () => {
    try {
      const res = await fetch(`https://localhost:7068/api/Ticket`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      const data = await res.json();
      setTickets(data.tickets || []);
    } catch (err) {
      console.error("Failed to fetch tickets:", err);
    }
  };

  useEffect(() => {
    fetchTickets();
  }, []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await fetch(`https://localhost:7068/api/Ticket`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(form),
      });

      const data = await res.json();

      if (res.ok) {
        setForm({ title: "", description: "" });
        fetchTickets(); // Refresh list
      } else {
        alert(data.message || "Ticket creation failed");
      }
    } catch (err) {
      alert("Error creating ticket");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const openTicketInNewTab = (ticketId) => {
    window.open(`/tickets/${ticketId}`, "_blank");
  };

  return (
    <div className="container py-4">
      <h2 className="mb-4">Create Ticket</h2>

      <form onSubmit={handleSubmit} className="mb-5">
        <div className="mb-3">
          <input
            type="text"
            name="title"
            className="form-control"
            placeholder="Ticket Title"
            value={form.title}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <textarea
            name="description"
            className="form-control"
            placeholder="Ticket Description"
            rows="4"
            value={form.description}
            onChange={handleChange}
            required
          />
        </div>

        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Submitting..." : "Submit Ticket"}
        </button>
      </form>

      <h2 className="mb-3">All Tickets</h2>

      {tickets.length > 0 ? (
        <ul className="nav nav-tabs flex-wrap">
          {tickets.map((ticket) => (
            <li className="nav-item" key={ticket._id}>
              <button
                className="nav-link"
                onClick={() => openTicketInNewTab(ticket._id)}
              >
                {ticket.title}
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <p>No tickets submitted yet.</p>
      )}
    </div>
  );
}
