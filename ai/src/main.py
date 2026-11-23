from fastapi import FastAPI
from embedding import file_embedding,vectorize_query
from db import add_db,search as serach_db
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI(title="smart-drive-ai")
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:5070",   
        "http://localhost:3000",
    ],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)
@app.get("/health")
def health():
    return {"status":"ok"}
@app.get("/search")
def search(query:str,col:str):
    query_vector = vectorize_query(query)
    output = serach_db(query_vector,col)
    return {"status":"ok",
            "data":output}
@app.post("/add")
def add(file_url:str,file_id:str,col:str):
    points = file_embedding(file_url,file_id)
    add_db(points=points,collection=col)
    return {"status":"ok"}

