import pandas as pd
import numpy as np

# Set random seed for reproducibility
np.random.seed(42)

# Generate sample dataset
num_matches = 1000  # Adjust as needed
data = []

for _ in range(num_matches):
    team_a_id = np.random.randint(1, 21)  # 20 teams
    team_b_id = np.random.randint(1, 21)
    
    while team_a_id == team_b_id:  # Ensure different teams
        team_b_id = np.random.randint(1, 21)

    # Generate statistics for Team A and Team B
    team_a_played = np.random.randint(20, 40)
    team_a_wins = np.random.randint(5, team_a_played // 2)
    team_a_loses = np.random.randint(0, team_a_played // 3)
    team_a_goals_for = np.random.randint(20, 80)
    team_a_goals_against = np.random.randint(15, 60)
    team_a_clean_sheets = np.random.randint(0, team_a_played // 3)
    team_a_failed_to_score = np.random.randint(0, team_a_played // 4)
    team_a_streak_wins = np.random.randint(0, 5)
    team_a_streak_loses = np.random.randint(0, 5)
    team_a_yellow_cards = np.random.randint(20, 60)
    team_a_red_cards = np.random.randint(0, 5)

    team_b_played = np.random.randint(20, 40)
    team_b_wins = np.random.randint(5, team_b_played // 2)
    team_b_loses = np.random.randint(0, team_b_played // 3)
    team_b_goals_for = np.random.randint(20, 80)
    team_b_goals_against = np.random.randint(15, 60)
    team_b_clean_sheets = np.random.randint(0, team_b_played // 3)
    team_b_failed_to_score = np.random.randint(0, team_b_played // 4)
    team_b_streak_wins = np.random.randint(0, 5)
    team_b_streak_loses = np.random.randint(0, 5)
    team_b_yellow_cards = np.random.randint(20, 60)
    team_b_red_cards = np.random.randint(0, 5)

    # Determine Match Result , <2
    match_result = np.random.randint(-1, 2)
    # if team_a_goals_for > team_b_goals_against:
    #     match_result = 1  # Team A wins
    # elif team_a_goals_for < team_b_goals_against:
    #     match_result = -1  # Team A loses
    # else:
    #     match_result = 0  # Draw

    # Append row to dataset
    data.append([
        team_a_id, team_a_played, team_a_wins, team_a_loses, team_a_goals_for, team_a_goals_against,
        team_a_clean_sheets, team_a_failed_to_score, team_a_streak_wins, team_a_streak_loses,
        team_a_yellow_cards, team_a_red_cards,
        team_b_id, team_b_played, team_b_wins, team_b_loses, team_b_goals_for, team_b_goals_against,
        team_b_clean_sheets, team_b_failed_to_score, team_b_streak_wins, team_b_streak_loses,
        team_b_yellow_cards, team_b_red_cards, match_result
    ])

# Convert to DataFrame
columns = [
    "TeamA_Id", "TeamA_Played", "TeamA_Wins", "TeamA_Loses", "TeamA_GoalsFor", "TeamA_GoalsAgainst",
    "TeamA_CleanSheets", "TeamA_FailedToScore", "TeamA_StreakWins", "TeamA_StreakLoses",
    "TeamA_YellowCardsTotal", "TeamA_RedCardsTotal",
    "TeamB_Id", "TeamB_Played", "TeamB_Wins", "TeamB_Loses", "TeamB_GoalsFor", "TeamB_GoalsAgainst",
    "TeamB_CleanSheets", "TeamB_FailedToScore", "TeamB_StreakWins", "TeamB_StreakLoses",
    "TeamB_YellowCardsTotal", "TeamB_RedCardsTotal", "Match_Result"
]

df = pd.DataFrame(data, columns=columns)

# Save dataset
df.to_csv("football_matches.csv", index=False)

print("Dataset saved as 'football_matches.csv'")

