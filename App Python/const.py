TIMESTAMP = "timestamp"
DATA_FORMAT = (
    "timestamp:time:o,value:value,sensor:obname[$],valuetype:vtname,unit:vtunit"
)
PREDICT_COLUMN = "predicted"
PREDICTION_FORMAT = "{col}"
RMSE_FORMAT = "RMSE"
MAPE_FORMAT = "MAPE (%)"

LAG_FORMAT = "{col}_lag{index}"
MAP_TN = {9: "1h", 10: "3h", 11: "6h", 12: "12h", 13: "1d", 14: "2d", 15: "4d"}

FORMAT_API = (
    "token={token}&st={st}&et={et}&ob={obs}&vt={vts}&pt={points}&rf={format}&tn={tn}"
)

FORMAT_MODEL_ID = "{workspace_id}_{name}_{model_type}.pkl"

OPERATION_REGEX = r"[A-Za-z_][\w\-|+|*|/|]*"