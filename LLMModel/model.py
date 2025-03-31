import pandas as pd
import pickle
import xgboost as xgb
import numpy as np
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score
from xgboost import XGBClassifier

# Load dataset
df = pd.read_csv("./football_matches.csv")


# Convert Match_Result labels
df["Match_Result"] = df["Match_Result"].map({-1: 2, 0: 0, 1: 1})  # Loss = 0, Draw = 1, Win = 2

# Extract target variable
y = df["Match_Result"].values
X = df.drop(columns=["Match_Result"])

# Debug: Check unique values in y
print("Unique values in y:", np.unique(y))  # Must show [0 1 2]

# Ensure all three classes exist in y
if len(np.unique(y)) < 3:
    raise ValueError("Dataset must contain all three classes: 0 (Loss), 1 (Draw), and 2 (Win).")

# Split dataset into training and testing
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42, stratify=y)

# Define pipeline for multi-class classification
pipeline = Pipeline([
    ('scaler', StandardScaler()),
    ('classifier', XGBClassifier(
        objective="multi:softmax", num_class=3, 
        n_estimators=300,  # More trees
        learning_rate=0.05,  # Lower learning rate
        max_depth=6,  # Increase depth for more complex relationships
        colsample_bytree=0.8,  # Feature selection
        subsample=0.8,  # Prevent overfitting
        eval_metric="mlogloss"
    ))
])

# Train pipeline
pipeline.fit(X_train, y_train)

# Evaluate model
y_pred = pipeline.predict(X_test)
result_map = {0: "X", 1: "1", 2: "2"}
predicted_results = [result_map[p] for p in y_pred]

# Display results
df_results = X_test.copy()
df_results["Predicted_Result"] = predicted_results
print(df_results.head())  # Show first 5 predictions

for i, row in df_results.head(10).iterrows():
    print(f"Match {i + 1}: TeamA:{row['TeamA_Id']} vs TeamB:{row['TeamB_Id']}  {row['Predicted_Result']}")

accuracy = accuracy_score(y_test, y_pred)
print("Model Accuracy:", accuracy)

# Count occurrences of each class in training data
train_classes, train_counts = np.unique(y_train, return_counts=True)
print("Training Data Distribution:", dict(zip(train_classes, train_counts)))

# Count occurrences of each class in testing data
test_classes, test_counts = np.unique(y_test, return_counts=True)
print("Testing Data Distribution:", dict(zip(test_classes, test_counts)))

# Save the trained model
with open("model.pkl", "wb") as f:
    pickle.dump(pipeline, f)

print("Model saved successfully!")



