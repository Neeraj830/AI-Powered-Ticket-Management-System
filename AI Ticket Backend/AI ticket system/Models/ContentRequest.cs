﻿namespace AI_ticket_system.Models
{
    public class ContentRequest
    {
        public Content[] contents { get; set; }
    }
    public class Content
    {
        public Part[] parts { get; set; }
    }
    public class Part
    {
        public string text { get; set; }
    }
}
