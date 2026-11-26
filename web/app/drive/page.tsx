"use client"

import FilesGrid from "@/components/files/filesGrid";
import FilesGridLoading from "@/components/files/filesGridLoading";
import { useAuthContext } from "@/context/AuthContext"
import { files as getFiles } from "@/lib/files"
import { FileDto } from "@/models/files";
import { useQuery } from "@tanstack/react-query";
import { BiSearch } from "react-icons/bi";
import { BiUpload } from "react-icons/bi";
import { uploadDone, uploadMedia, uploadURL } from "@/lib/files";
import { useRef, useState } from "react";
export default function Page() {
    
    const { isLoggedIn, getValidToken, user, loading } = useAuthContext();
    const [files, setFiles] = useState<File[]>([]);
    const inputRef = useRef<HTMLInputElement>(null);
    const handleClick = () => {
        inputRef.current?.click(); 
    };

    const handleChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const fileList = e.target.files;
        if (!fileList) return;

        const arr = Array.from(fileList);
        setFiles(arr);

        const token = await getValidToken(); 

        for (const file of arr) {
            const res = await uploadMedia(token!, file.name, file.size, file.type);
            console.log(res);
            const fileId = res.data.fileId;
            const url = res.data.url;
            console.log(fileId)
            console.log(url)

            const res2 = await uploadURL(file,url,file.type,file.size);
            console.log(res2);
        }
    };
    const { data, isLoading, isError } = useQuery({
        queryKey: ["files"],
        enabled: !loading && isLoggedIn,
        queryFn: async () => {
            const token = await getValidToken();
            if (!token) throw new Error("Brak Sesji");
            return getFiles(token);
        }
    });
    
    const showSkeleton = loading || isLoading;

    if (!loading && !isLoggedIn) {
        return <div>Co ty gosciu</div>;
    }

    

   return (
        <div className="relative h-screen w-full  flex flex-col items-center gap-4 pt-10 ">
            <div onClick={handleClick} className="group absolute top-4 right-4 w-12 h-12 bg-sky-500/40 hover:bg-sky-500 rounded-full flex items-center justify-center hover:w-24 transition-all cursor-pointer z-10">
                <BiUpload className="text-white/40 text-3xl group-hover:hidden" />
                <span className="hidden text-white text-xl group-hover:block"><BiUpload className="text-white text-3xl " /></span>
                 <input
                    ref={inputRef}
                    type="file"
                    onChange={handleChange}
                    style={{ display: "none" }}
                    multiple
                />
            </div>

            {showSkeleton  && (
                <div className="flex-1 min-h-0 w-full flex flex-col text-white  p-4 ">
                    <FilesGridLoading />
                </div>
            )}
            {isError && <p>Błąd pobierania plików</p>}

            {data && (
                <div className="flex-1 min-h-0 w-full flex flex-col text-white  p-4 ">
                    <FilesGrid files={data.data as FileDto[]} />
                </div>
            )}

            <div className="shrink-0 bg-neutral-600 h-14 w-4/5 rounded-full py-0.5 my-8 flex">
                <input
                    className="flex-1 min-w-0 text-white outline-none h-full rounded-full p-4 ml-3"
                    placeholder="type prompt..."
                    type="text"
                />
                <div className="h-full flex justify-end items-center px-4">
                    <div className="w-14 h-10 rounded-full flex justify-center items-center bg-sky-500">
                        <BiSearch className="text-white text-3xl" />
                    </div>
                </div>
            </div>
        </div>
    );
}