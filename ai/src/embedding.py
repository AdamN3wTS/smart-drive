from fastembed import TextEmbedding
import httpx
import uuid
import io
import pymupdf
import base64
from pypdf import PdfReader
from llm import get_image_description

from qdrant_client.models import  PointStruct

model = TextEmbedding(model_name="google/embeddinggemma-300m",threads=1,cache_dir="./fastembed_cache")
def get_file_pdf(url):
    r = httpx.get(url,timeout=60)
    r.raise_for_status()
    data=r.content
    doc = pymupdf.open(stream=data, filetype="pdf")
    return doc
def get_file_image(url):
    r = httpx.get(url,timeout=60)
    r.raise_for_status()
    image_b64 = base64.b64encode(r.content).decode("utf-8")
    return image_b64

def get_chunks(file,file_id,chunk_size=600,overlap=200):
    chunks =[]
    for i,page in enumerate(file):
        text = page.get_text()
        if not text:
            continue
        step = chunk_size-overlap
        for j in range(0,len(text),step):
            chunk= text[j:chunk_size+j]
            if len(chunk.strip())>40:
                chunks.append({
                    "text":chunk,
                    "page":i,
                    "source":file_id
                })
    return chunks
def file_embedding(url,id,content_type:str):
    value = content_type.split("/")[0]
    match value:
        case "application":
            return file_embedding_pdf(url,id)
        case "image":
            return file_embedding_image(url,id,content_type)
def file_embedding_image(url,id,content_type):
    print("Mamy juz base64")
    file_base64 = get_file_image(url)
    print("zaczynam text gemini: ")

    text = get_image_description(file_base64,content_type)
    doc = f"title: none | text: {text}"
    print("mamy juz text z gemini: ",text)
    vector = list(model.embed([doc]))[0]
    points =[]
    
    id=str(uuid.uuid4())
    payload = {"text":text,"page":0,"source":id}
    points.append(PointStruct(
        id=id,
        vector=vector.tolist(),
        payload=payload
    ))
    return points
def file_embedding_pdf(url,id):
    file = get_file_pdf(url)
    chunks = get_chunks(file,id)
    texts = [c["text"] for c in chunks]
    vectors = list(model.embed(texts))
    points =[]
    for chunk,vector in zip(chunks,vectors):
        id=str(uuid.uuid4())
        payload = chunk
        points.append(PointStruct(
            id=id,
            vector=vector.tolist(),
            payload=payload
        ))
    return points
def vectorize_query(query):
    text = f"task: search result | query: {query}"
    return list(model.embed([text]))[0].tolist()

    

        

