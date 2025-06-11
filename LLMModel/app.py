from os import access
from fastapi import FastAPI, UploadFile, File
import pandas as pd
import io 
import json
from model import train_model_from_df
from model_predictor import predict_match_results_from_csv

app = FastAPI()

@app.post("/train")
async def train_model(file: UploadFile = File(...)):
    try:
        # Read the uploaded file contents
        contents = await file.read()
        df = pd.read_csv(io.BytesIO(contents))  # Read CSV from the file

        # Train the model
        accuracy = train_model_from_df(df)

        # Return status and accuracy                                                                                                
        return {"status": "Model trained successfully", "accuracy": accuracy}

    except Exception as e:
        # Handle errors gracefully
        return {"status": "Error", "message": str(e)}



@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    contents = await file.read()
    df_with_predictions = predict_match_results_from_csv(contents)

    # return the json 
    return {"records": df_with_predictions}

