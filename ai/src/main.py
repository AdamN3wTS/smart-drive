from fastapi import FastAPI

app = FastAPI(title="smart-drive-ai")
@app.get("/health")
def health():
    return {"status":"ok"}
@app.get("/search")
def search():
    return {"status":"ok"}
@app.post("/add")
def add():
    return {"status":"ok"}

