"use client"

import { UseAuthContext } from "@/context/AuthContext"
import { useState } from "react"
import { login as loginApi, me as meApi } from "@/lib/auth"
import { redirect, useRouter } from "next/navigation";
export default function Page(){
    const [name,setName] = useState("")
    const [password,setPassword] = useState("")
    const {loginContext} = UseAuthContext();
    const buttonHandler = async () =>{
        if (name != "" && password != ""){
            const apiResponse = await loginApi(name,password);
            const meResponse = await meApi(apiResponse.data.accessToken!);
            loginContext(apiResponse.data.accessToken!,meResponse.data);
            redirect("/")
        }
        return
    
    }
    return(<div className="min-h-screen min-w-screen border-4 flex flex-col justify-center items-center border-amber-400">
        <div className="w-full h-full items-center justify-center flex flex-col gap-6">
            
            <div >
                <input  className={`outline-none focus:outline-none focus:ring-0 focus:border-violet-600 focus:bg-neutral-50 rounded-full w-60 border-2 px-4 py-2 text-md border-violet-400`} placeholder={"Login"} value={name} onChange={(e)=>setName(e.target.value)} type="text" />
            </div>
            <div>
                <input className={`outline-none focus:outline-none focus:ring-0 focus:border-violet-600 focus:bg-neutral-50 rounded-full w-60 border-2 px-4 py-2 text-md border-violet-400`} placeholder={"Password"} value={password} onChange={(e)=>setPassword(e.target.value)} type="password" />
            </div>
            <div className="py-4">
                <button onClick={buttonHandler} className="w-40 h-12 rounded-full border-2 border-violet-400 cursor-pointer hover:bg-violet-400 hover:text-white transition-all">Login</button>
            </div>
        </div>
    </div>)
}