# model_predictor.py
import pandas as pd
import pickle
import io

# Load model once
with open("optimized_model.pkl", "rb") as f:
    model = pickle.load(f)

result_map = {0: "X", 1: "1", 2: "2"}

def predict_match_results_from_csv(csv_bytes: bytes) -> pd.DataFrame:
    df = pd.read_csv(io.BytesIO(csv_bytes))

    # Drop ID and label columns if they exist
    original_ids = df[["TeamA_TeamId", "TeamB_TeamId"]].copy()
    # df.drop(columns=["TeamA_TeamId", "TeamB_TeamId"], errors="ignore", inplace=True)
    df.drop(columns=["Match_Result"], errors="ignore", inplace=True)

    # Apply same feature engineering
    df["TeamA_GoalDifference"] = df["TeamA_GoalsFor"] - df["TeamA_GoalsAgainst"]
    df["TeamB_GoalDifference"] = df["TeamB_GoalsFor"] - df["TeamB_GoalsAgainst"]
    df["TeamA_WinRatio"] = df["TeamA_Wins"] / df["TeamA_Played"]
    df["TeamB_WinRatio"] = df["TeamB_Wins"] / df["TeamB_Played"]
    df["TeamA_LossRatio"] = df["TeamA_Loses"] / df["TeamA_Played"]
    df["TeamB_LossRatio"] = df["TeamB_Loses"] / df["TeamB_Played"]

    df.drop(columns=["TeamA_TeamId", "TeamB_TeamId"], errors="ignore", inplace=True)

    # Predict
    predictions = model.predict(df)

    result_df = original_ids.copy()
    result_df["Predicted_Result"] = [result_map[p] for p in predictions]

    #return df as json
    # Convert DataFrame to JSON
    json_result = result_df.to_dict(orient="records")

    return json_result
