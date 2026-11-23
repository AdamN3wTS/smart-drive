import { safeJson } from "./fetchHelper";

const url = process.env.NEXT_PUBLIC_API_URL;

export async function login(name:string,password:string){
    if(!url) return null;
    const req = await fetch(`${url}/auth/login`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json"
        },
        credentials:"include",
        body: JSON.stringify({NameOrEmail:name,Password:password})
    })
    return safeJson(req);
}
export async function register(name:string,email:string,password:string,confirmPassword:string) {
    if(!url) return null;
    const req = await fetch(`${url}/auth/register`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json"
        },
        credentials:"include",
        body: JSON.stringify({Name:name,Email:email,Password:password,ConfirmPassword:confirmPassword})
    })
    return safeJson(req);
}
export async function logout(token:string){
    if(!url) return null;
    const req = await fetch(`${url}/auth/logout`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json"
        },
        credentials:"include",
    })
    return safeJson(req);
}
export async function refresh(){
    if(!url) return null;
    const req = await fetch(`${url}/auth/refresh`,{
        method:'POST',
        headers:{
            "Content-Type": "application/json",
        },
        credentials:"include"
    })
    return safeJson(req);
}
export async function me(token:string){
    if(!url) return null;
    const req = await fetch(`${url}/auth/me`,{
        method:'GET',
        headers:{
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
    })
    return safeJson(req);
}
export function loginGoogle(){
    window.location.href = `${url}/auth/google`;
}