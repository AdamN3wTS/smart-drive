import { safeJson } from "./fetchHelper";

const url = `${process.env.NEXT_PUBLIC_API_URL}/api`;
export async function files(token:string){
    if(!url) return null;
        const req = await fetch(`${url}/files/me`,{
            method:'GET',
            headers:{
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
        })
    return safeJson(req);
}


export  async function uploadMedia(token:string,name:string,length:number,contentType:string){
    if(!url) return null;
    const req = await fetch(`${url}/files/upload`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body:JSON.stringify({Name:name,ContentLength:length,ContentType:contentType})
        
    })
    console.log('upload',req);
    return safeJson(req);
}
export  async function uploadDone(fileUrl:string,addedById:string){    
    if(!url) return null;
    const req = await fetch(`${url}/files/upload-done`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json",
        },
        body:JSON.stringify({AddedById:addedById,FileUrl:fileUrl})
        
    })
    return safeJson(req);
}
export  async function uploadURL(file:File,url2:string,type:string,length:number){    
    console.log(url2);
    const req = await fetch(`${url2}`,{
        method:'PUT',
        headers:{
            "Content-Type": `${type}`,
        },
        body:file
        
    })
    console.log('req',req);
    return safeJson(req);
}