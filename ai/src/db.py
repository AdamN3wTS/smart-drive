import os
from qdrant_client import QdrantClient
from qdrant_client.models import VectorParams, Distance, PointStruct
from dotenv import load_dotenv
load_dotenv()
QDRANT_APIKEY=os.getenv("QDRANT_APIKEY")
QDRANT_URL=os.getenv("QDRANT_URL")
client = QdrantClient(url=QDRANT_URL,api_key=QDRANT_APIKEY)
def init_collection(col):
    if not client.collection_exists(col):
        client.create_collection(collection_name=col,
                                vectors_config=VectorParams(size=768,distance=Distance.COSINE))

def add_db(points,collection):
    init_collection(collection)
    client.upsert(collection_name=collection,points=points)
def search(query_vecotor,collection,limit=3):
    response = client.query_points(
        collection_name=collection,
        query=query_vecotor,
        limit=limit
    )
    return [
        {
            "score": hit.score,
            "page": hit.payload.get("page") if hit.payload else None,
            "text": hit.payload.get("text") if hit.payload else None,
            "source": hit.payload.get("source") if hit.payload else None
        }
        for hit in response.points
    ]

