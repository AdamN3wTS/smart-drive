from fastembed import TextEmbedding
import httpx
import uuid
import io
import pymupdf

from pypdf import PdfReader

from qdrant_client.models import  PointStruct

model = TextEmbedding(model_name="google/embeddinggemma-300m",threads=1,cache_dir="./fastembed_cache")
def get_file(url):
    r = httpx.get(url,timeout=60)
    r.raise_for_status()
    data=r.content
    doc = pymupdf.open(stream=data, filetype="pdf")
    return doc
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
def file_embedding(url,id):
    file = get_file(url)
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

    

        

