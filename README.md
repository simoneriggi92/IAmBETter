# ⚽ IAmBETter – Serie A Match Predictor

**IAmBETter** is an AI-powered web application that predicts outcomes of Serie A football matches using historical statistics. It uses a trained machine learning model to generate match results (`1`, `X`, `2`), stores predictions in a MongoDB database, and displays them in a live-updated UI.

---

## 🎯 Project Objective

The aim of this project is to build an automated Serie A match prediction service. It serves as an MVP (Minimum Viable Product) to experiment with:

* Realistic architecture for an AI-integrated .NET + Python system
* FastAPI-based ML model training pipeline using XGBoost
* Communication between .NET services and Python services

All training data is retrieved from a third-party API provider (**API-Football**) and assembled into spreadsheets representing historical 1vs1 matches. The more stats are available, the better the accuracy of the model.

The match simulator, in its current form, returns:

* `1` → Home team win
* `X` → Draw
* `2` → Away team win

This project does not aim to be a fully-fledged simulator but instead focuses on building a robust backend infrastructure and exploring AI potential.

---

## 🏗️ Architecture Overview

The architecture includes:

* **ASP.NET Core backend** (.NET 8) with Razor Pages frontend
* **FastAPI service** in Python for ML training and prediction
* **MongoDB** for persisting predictions, matches, and team data

At startup, a TaskManager runs two scheduled services:

### 🔄 `PredictionServiceTask`

* Fetches league info (teams, rounds, matches) from API-Football
* Populates MongoDB collections if data is missing
* Waits up to 2 minutes to comply with 10-req/min API limit
* Checks if the match is played and creates/updates training dataset
* Calls the `/train` endpoint to train the ML model
* Calls the `/predict` endpoint with upcoming matches to receive predictions

### 📜 `HistoryServiceTask`

* Runs daily to verify prediction accuracy
* Compares predicted vs actual outcomes
* Marks predictions as successful or failed
* Archives completed predictions in a historical collection

---

## 🚀 Features

* ⚽ Predicts match results using team statistics (goals, wins, losses, etc.)
* 🧠 Model training using historical match data (CSV)
* 📦 MongoDB-backed data persistence for teams, matches, and predictions
* 🔄 Live UI refresh for real-time prediction updates
* 📊 Historical prediction performance with success/failure tracking
* 🔌 REST API for uploading datasets and generating predictions
* 🖥️ Admin view for training, inspecting, and monitoring prediction results

---

## 📂 Project Structure

```plaintext
IAmBetter/
│
├── app/                      # FastAPI backend for model training and prediction
│   └── model_predictor.py    # Loads ML model and exposes prediction functions
│
├── iambetter/                # ASP.NET Razor Pages frontend
│   ├── Pages/
│   │   └── Index.cshtml      # Main prediction UI
│   ├── Services/             # API/Mongo/Prediction services
│   └── Program.cs            # Application startup
│
├── Domain/                   # Models & DTOs
│   ├── MatchDTO.cs
│   ├── PredictionDTO.cs
│   └── Team.cs
│
├── MongoDB/                  # Collections: Matches, Predictions, Teams
│
├── optimized_model.pkl       # Trained scikit-learn model (stored locally)
│
└── README.md                 # This file
```

**🛠️ Technologies**

* **Frontend**: ASP.NET Core Razor Pages
* **Backend (ML)**: Python + FastAPI
* **Database**: MongoDB (hosted or local)
* **ML Library**: XGBoost + Scikit-learn
* **Communication**: HTTP API between C# and Python services

---

## ⚙️ How It Works

### 🔧 Model Training

1. Upload a `.csv` file with match statistics (excluding result).
2. Python backend trains the model using XGBoost.
3. Optimized model is saved as `optimized_model.pkl`.

### 🔍 Prediction Generation

1. C# backend fetches new match data to predict.
2. Data is sent to FastAPI `/predict` endpoint.
3. Predictions are saved to MongoDB.
4. UI refreshes automatically every 60 seconds.

### 📊 Viewing Results

* **Current Predictions**: Displayed with teams, dates, results, accuracy.
* **Prediction History**: Archived with color-coded success/failure.

---

## 🧪 API Endpoints

### Model Training

```
POST /train
Content-Type: multipart/form-data
Body: CSV file with historical match data
```

### Match Prediction

```
POST /predict
Content-Type: multipart/form-data
Body: CSV file with upcoming matches
```

---

## 🧠 Model Training Pipeline

1. **Load Dataset**

```python
df = pd.read_csv("football_matches.csv")
```

2. **Label Encoding**

```python
df["Match_Result"] = df["Match_Result"].map({-1: 2, 0: 0, 1: 1})
```

3. **Feature Engineering**: Adds win/loss ratios and goal differences
4. **Drop Irrelevant Columns**: `TeamA_Id`, `TeamB_Id`
5. **Define Features & Target**: `X` and `y`
6. **Handle Imbalance with SMOTE**
7. **Split Data**: 80% train / 20% test
8. **Hyperparameter Tuning** using `GridSearchCV`
9. **Train Pipeline** with `StandardScaler` and best `XGBClassifier`
10. **Evaluate**: Accuracy, classification report, top 10 predictions
11. **Feature Importance Plot** using `matplotlib`
12. **Save Model** as `optimized_model.pkl`

---

## 🖼️ UI Overview

* 🔄 **Live Prediction Table**: Updated every 60 seconds with new results.
* 🧾 **Prediction History Table**: Shows actual vs predicted results per round.

  * ✅ Green = correct prediction
  * ❌ Red = failed prediction

### 📈 Example Prediction

```
Match               Predicted   Actual   Status
Inter vs Milan      1           1        ✅ Success
Lazio vs Juventus   X           2        ❌ Failed
```

---

## 📌 Prerequisites

* Python 3.9+
* .NET 8+
* MongoDB (local or Atlas)
* Node.js (optional, for frontend build)

---

## 🚀 Getting Started

### Clone the repository:

```bash
git clone https://github.com/your-username/iambetter.git
```

### Start the Python backend:

```bash
cd app
uvicorn app:app --reload --host 0.0.0.0 --port 8000
```

### Start the .NET frontend:

```bash
dotnet run --project iambetter
```

### Open in browser:

```
http://localhost:5000
```

---

## 🐳 Docker Setup

Start MongoDB with Docker:

```bash
docker run -d --name mongodb -p 27017:27017 mongo:latest --bind_ip_all
```

> *Note: MongoDB Community Server may not run reliably on Windows.*

---

## 📌 TODO

* [ ] Improve XGBoost model to return predictions more complex than simple 1, X, 2
* [ ] Add authentication
* [ ] Add match date filters
* [ ] Support multiple leagues
* [ ] Export predictions to CSV

---

## 📄 License

MIT License – Free to use, modify, and distribute.

---

## 🤝 Contributing

Pull requests are welcome! Feel free to open an issue or contribute improvements to model accuracy, UI, or data ingestion workflows.

---

## 📌 Final Notes

This MVP was built for experimentation and understanding:

* XGBoost model training on football data
* Scheduled background processing (.NET Hosted Services)
* Free-tier API integration with rate limits
* Real-time frontend for viewing results

Designed to be simple but scalable, with strong future potential.
