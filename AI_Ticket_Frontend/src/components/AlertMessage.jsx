import React from "react";

export default function AlertMessage({ message, type, onClose }) {
  if (!message) return null;

  return (
    <div className="position-fixed top-0 end-0 p-3" style={{ zIndex: 9999 }}>
      <div
        className={`toast show align-items-center text-white bg-${type} border-0 shadow`}
        role="alert"
      >
        <div className="d-flex">
          <div className="toast-body">{message}</div>
          <button
            type="button"
            className="btn-close btn-close-white me-2 m-auto"
            onClick={onClose}
          ></button>
        </div>
      </div>
    </div>
  );
}
