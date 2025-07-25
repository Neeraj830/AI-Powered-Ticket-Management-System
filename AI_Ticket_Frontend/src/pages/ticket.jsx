import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import ReactMarkdown from "react-markdown";

export default function TicketDetailsPage() {
  const { id } = useParams();
  const [ticket, setTicket] = useState(null);
  const [loading, setLoading] = useState(true);

  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchTicket = async () => {
      try {
        const res = await fetch(`https://localhost:7068/api/Ticket/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const data = await res.json();

        if (res.ok) {
          setTicket(data.ticket);
        } else {
          alert(data.message || "Failed to fetch ticket");
        }
      } catch (err) {
        console.error(err);
        alert("Something went wrong");
      } finally {
        setLoading(false);
      }
    };

    fetchTicket();
  }, [id]);

  if (loading)
    return (
      <div className="d-flex justify-content-center align-items-center vh-100">
        <div className="text-center">
          <div className="spinner-border text-primary mb-2" role="status" />
          <div>Loading ticket details...</div>
        </div>
      </div>
    );

  if (!ticket)
    return (
      <div className="text-center mt-5 text-danger fw-semibold">
        Ticket not found
      </div>
    );

  return (
    <div className="container mt-4">
      <h2 className="mb-4 text-center text-primary fw-bold">Ticket Details</h2>

      <div className="card shadow-sm border-0">
        <div className="card-body">
          <h4 className="card-title mb-3 text-dark">{ticket.title}</h4>
          <p className="card-text">{ticket.description}</p>

          <hr className="my-4" />

          <h5 className="mb-3">Metadata</h5>
          <ul className="list-group list-group-flush">
            <li className="list-group-item">
              <strong>Status:</strong> {ticket.status}
            </li>

            {ticket.priority && (
              <li className="list-group-item">
                <strong>Priority:</strong> {ticket.priority}
              </li>
            )}

            {ticket.relatedSkills?.length > 0 && (
              <li className="list-group-item">
                <strong>Related Skills:</strong>{" "}
                {ticket.relatedSkills.join(", ")}
              </li>
            )}

            {ticket.helpfulNotes && (
              <li className="list-group-item">
                <strong>Helpful Notes:</strong>
                <div className="border rounded p-3 mt-2 bg-light text-dark-emphasis">
                  <ReactMarkdown>{ticket.helpfulNotes}</ReactMarkdown>
                </div>
              </li>
            )}

            {ticket.assignedTo?.email && (
              <li className="list-group-item">
                <strong>Assigned To:</strong> {ticket.assignedTo.email}
              </li>
            )}

            {ticket.createdAt && (
              <li className="list-group-item text-muted small">
                Created At: {new Date(ticket.createdAt).toLocaleString()}
              </li>
            )}
          </ul>
        </div>
      </div>
    </div>
  );
}
