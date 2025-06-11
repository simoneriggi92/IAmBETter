# IAamBETter– Serie A Match Predictor

**IAmBetter** is an AI-powered web application that predicts outcomes of Serie A football matches using historical statistics. It uses a trained machine learning model to generate match results (1, X, 2), stores predictions in a MongoDB database, and displays them in a live-updated UI.

---

## 🚀 Features

- ⚽ Predicts match results using team statistics (goals, wins, losses, etc.)
- 🧠 Model training using historical match data (CSV)
- 📦 MongoDB-backed data persistence for teams, matches, and predictions
- 🔄 Live UI refresh for real-time prediction updates
- 📊 Historical prediction performance with success/failure tracking
- 🔌 REST API for uploading datasets and generating predictions
- 🖥️ Admin view for training, inspecting, and monitoring prediction results

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
🛠️ Technologies
Frontend: ASP.NET Core Razor Pages

```
Backend (ML): Python + FastAPI

Database: MongoDB (hosted or local)

ML Library: Scikit-learn

Communication: HTTP API between C# and Python services

## ⚙️ How It Works
Training the Model
Upload a .csv file with match statistics (excluding result).

The Python backend trains or updates the model.

The optimized model is saved as optimized_model.pkl.

Generating Predictions
The C# frontend fetches new match data to predict.

Data is sent to the FastAPI prediction endpoint.

Predictions are returned and saved into MongoDB.

UI updates every minute to reflect the latest predictions.

Viewing Results
Current Predictions: Automatically fetched and displayed in a table.

Prediction History: Displayed with success/failure status per round.

## 🧪 API Endpoints
Model Training
http POST /train
Content-Type: multipart/form-data
Body: CSV file with historical match data

Prediction
httpPOST /predict
Content-Type: multipart/form-data
Body: CSV file with upcoming matches

## 🖼️ UI Overview
🔄 Live Prediction Table: Updated every 60 seconds with new results.

🧾 Prediction History Table: Shows actual vs predicted results per round.

✅ Green rows = correct prediction

❌ Red rows = failed prediction

## 📈 Example Prediction
Match	Predicted	Actual	Status
Inter vs Milan	1	1	✅ Success
Lazio vs Juventus	X	2	❌ Failed

## 🧰 Prerequisites
Python 3.9+

.NET 7+

MongoDB (local or Atlas)

Node.js (for building frontend, optional)

## 🚀 Getting Started
Clone the repo:

bash
git clone https://github.com/your-username/iambetter.git
Start the Python backend:

bash
cd app
uvicorn app:app --reload --host 0.0.0.0 --port 8000
Start the C# frontend:

bash
dotnet run --project iambetter
Open your browser:

http://localhost:5000

## 📌 TODO
 Add authentication

 Add match date filters

 Support multiple leagues

 Export predictions to CSV

## 📄 License
MIT License – Free to use, modify, and distribute.

## 🤝 Contributing
PRs are welcome! If you want to contribute to improving the predictions, UI, or dataset ingestion, feel free to open an issue or pull request.

yaml

---

Let me know if you want to include screenshots, diagrams, or setup instructions for Docker or deployment.








## Docker setup


Use the following command to add pull mongodb image and start container locally to the port 27017. 

```
docker run -d --name mongodb -p 27017:27017 mongo:latest --bind_ip_all

```


_Please note: mongo db community server was not working on windows machine_ 
