﻿using ExerciseWebApp.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ExerciseWebApp.Services
{
    public class LocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<T?> GetItemAsync<T>(string key)
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async ValueTask SetItemAsync<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        public async ValueTask RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task SeedDataAsync<T>(string key, T data)
        {
            var existingData = await GetItemAsync<T>(key);

            if (existingData == null)
            {
                await SetItemAsync(key, data);
            }
            else
            {
                await RemoveItemAsync(key);
                await SetItemAsync(key, existingData);
            }
        }

        public async Task<Workout> GetOneWorkoutAsync(int id)
        {
            var workouts = await GetItemAsync<List<Workout>>("workouts");
            var workout = workouts.Where(w => w.Id == id).FirstOrDefault();
            return workout;
        }

        public async ValueTask AddWorkoutAsync(Workout newWorkout)
        {
            List<Workout> existingWorkouts = await GetItemAsync<List<Workout>>("workouts") ?? new List<Workout>();
            if (existingWorkouts.Count == 0)
                newWorkout.Id = 1;
            else
            {
                var lastWorkout = existingWorkouts.Last();
                newWorkout.Id = lastWorkout.Id + 1;
            }
            existingWorkouts.Add(newWorkout);
            await SetItemAsync("workouts", existingWorkouts);
        }

        public async ValueTask UpdateWorkoutAsync(int id, Workout newWorkout)
        {
            var workouts = await GetItemAsync<List<Workout>>("workouts");
            var toBeUpdatedWorkout = await GetOneWorkoutAsync(id);
            workouts.Remove(toBeUpdatedWorkout);
            newWorkout.Id = id;
            workouts.Add(newWorkout);
            await SetItemAsync("workouts", workouts);
        }

        public async ValueTask DeleteWorkoutAsync(int id)
        {
            var workouts = await GetItemAsync<List<Workout>>("workouts");
            var toBeDeletedWorkout = await GetOneWorkoutAsync(id);
            workouts.Remove(toBeDeletedWorkout);
            await SetItemAsync("workouts", workouts);
        }

        public async ValueTask SaveWorkoutHistoryAsync(WorkoutHistory wHistory)
        {
            List<WorkoutHistory> workoutHistories = await GetItemAsync<List<WorkoutHistory>>("workoutHistories") ?? new List<WorkoutHistory>();
            if (workoutHistories.Count == 0)
            {
                   wHistory.Id = 1;
            }
            else
            {
                var lastWorkoutHistory = workoutHistories.Last();
                wHistory.Id = lastWorkoutHistory.Id + 1;
            }
            workoutHistories.Add(wHistory);
            await SetItemAsync("workoutHistories", workoutHistories);
        }

        public async Task<WorkoutHistory> GetOneWorkoutHistoryAsync(int id)
        {
            var workoutHistories = await GetItemAsync<List<WorkoutHistory>>("workoutHistories");
            var workoutHistory = workoutHistories.Where(w => w.Id == id).FirstOrDefault();
            return workoutHistory;
        }

        public async Task<List<WorkoutHistory>> GetAllWorkoutHistoriesAsync()
        {
            var workoutHistories = await GetItemAsync<List<WorkoutHistory>>("workoutHistories");
            return workoutHistories;
        }
    }
}
