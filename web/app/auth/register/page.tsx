"use client"

import { useState } from "react"

export default function Page(){
    const [name,setName] = useState("")
    const [password,setPassword] = useState("")

    return(<div className="min-h-screen min-w-screen border-4 border-amber-400">
            <div className="relative top-0 right-0">
                SmartDrive
            </div>
        <div className="w-full h-full items-center justify-center flex flex-col gap-6">
            
            <div >
                <input placeholder={"Login"} value={name} onChange={(e)=>setName(e.target.value)} type="text" />
            </div>
            <div>
                <input value={password} onChange={(e)=>setPassword(e.target.value)} type="password" />
            </div>
        </div>
    </div>)
}