import pandas as pd
import pickle
import xgboost as xgb
import numpy as np
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split, GridSearchCV
from sklearn.metrics import accuracy_score, classification_report
from xgboost import XGBClassifier
from imblearn.over_sampling import SMOTE

# 1️⃣ Load dataset
df = pd.read_csv("D:/source/iambetter/LLMModel/football_matches.csv")

# 2️⃣ Convert Match_Result labels
df["Match_Result"] = df["Match_Result"].map({-1: 2, 0: 0, 1: 1})  # Loss = 0, Draw = 1, Win = 2

# 3️⃣ Feature Engineering
df["TeamA_GoalDifference"] = df["TeamA_GoalsFor"] - df["TeamA_GoalsAgainst"]
df["TeamB_GoalDifference"] = df["TeamB_GoalsFor"] - df["TeamB_GoalsAgainst"]

df["TeamA_WinRatio"] = df["TeamA_Wins"] / df["TeamA_Played"]
df["TeamB_WinRatio"] = df["TeamB_Wins"] / df["TeamB_Played"]

df["TeamA_LossRatio"] = df["TeamA_Loses"] / df["TeamA_Played"]
df["TeamB_LossRatio"] = df["TeamB_Loses"] / df["TeamB_Played"]

# df["TeamA_PenaltyEffectiveness"] = df["TeamA_PenaltiesScored"] / (df["TeamA_Played"] + 1)
# df["TeamB_PenaltyEffectiveness"] = df["TeamB_PenaltiesScored"] / (df["TeamB_Played"] + 1)

# Drop team IDs (not useful for predictions)
df.drop(columns=["TeamA_Id", "TeamB_Id"], inplace=True)

# 4️⃣ Extract target variable
y = df["Match_Result"].values
X = df.drop(columns=["Match_Result"])

# 5️⃣ Handle Class Imbalance with SMOTE
smote = SMOTE(sampling_strategy="auto", random_state=42)
X_resampled, y_resampled = smote.fit_resample(X, y)

# 6️⃣ Split dataset into training and testing
X_train, X_test, y_train, y_test = train_test_split(X_resampled, y_resampled, test_size=0.2, random_state=42, stratify=y_resampled)

# 7️⃣ Hyperparameter Tuning using GridSearchCV
params = {
    "n_estimators": [100, 300, 500],
    "max_depth": [4, 6, 8],
    "learning_rate": [0.01, 0.05, 0.1],
    "subsample": [0.7, 0.8, 0.9],
    "colsample_bytree": [0.7, 0.8, 0.9]
}

xgb_model = XGBClassifier(objective="multi:softmax", num_class=3, eval_metric="mlogloss")
grid_search = GridSearchCV(xgb_model, param_grid=params, scoring="accuracy", cv=3, verbose=1)
grid_search.fit(X_train, y_train)

print("Best parameters:", grid_search.best_params_)

# 8️⃣ Define optimized pipeline
pipeline = Pipeline([
    ('scaler', StandardScaler()),
    ('classifier', grid_search.best_estimator_)  # Use the best XGBoost model found
])

# 9️⃣ Train pipeline
pipeline.fit(X_train, y_train)

# 🔟 Evaluate model
y_pred = pipeline.predict(X_test)
result_map = {0: "X", 1: "1", 2: "2"}
predicted_results = [result_map[p] for p in y_pred]

# Display first 10 predictions
df_results = X_test.copy()
df_results["Predicted_Result"] = predicted_results
print(df_results.head(10))

# Show evaluation metrics
print(classification_report(y_test, y_pred, target_names=["Loss", "Draw", "Win"]))
accuracy = accuracy_score(y_test, y_pred)
print("Model Accuracy:", accuracy)

# 🔥 Save the trained model
with open("optimized_model.pkl", "wb") as f:
    pickle.dump(pipeline, f)
print("Optimized model saved successfully!")

# ✅ Load and
