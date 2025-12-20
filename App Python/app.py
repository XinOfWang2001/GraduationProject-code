"""This is the main entry point when starting up a FastAPI service"""

import logging
import os

from dotenv import load_dotenv
from fastapi import FastAPI

from controllers import data_operation_controller, model_operation_controller

## Startup via normale command:
### fastapi dev app.py
app = FastAPI()
load_dotenv()

CHECK_ENV = os.getenv("CHECK_ENV", "== Niets ingesteld ==")

logging.basicConfig(
    format="{asctime} - {levelname} - {message}",
    style="{",
    datefmt="%Y-%m-%d %H:%M",
    level=logging.INFO)

CHECK_ENV = os.getenv("CHECK_ENV", "== Niets ingesteld ==")

logging.info(f"CHECK ENV == {CHECK_ENV}")

app.include_router(data_operation_controller.router)
app.include_router(model_operation_controller.router)

@app.get("/check")
async def check():
    logging.info("Check works")
    return f"It works! {CHECK_ENV}."
