﻿namespace ExerciseWebApp.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoLink { get; set; }
        public bool IsChecked { get; set; } = false;
        public bool IsDone { get; set; } = false;
    }
}
