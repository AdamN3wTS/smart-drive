import { FileDto } from "@/models/files";

export default function FilesGrid({files} : {files:FileDto[]}) {
    return (
        <div className="flex-1 min-h-0 w-full overflow-y-auto ">
            <div className="grid grid-cols-2">
                {files.map((_, i) => (
                    <div key={i} className="h-40  p-4">
                        <div className="w-full h-full border border-green-300" />
                    </div>
                ))}
            </div>
        </div>
    );
}