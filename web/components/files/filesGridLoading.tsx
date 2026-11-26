export default function FilesGridLoading() {
    return (
        <div className="flex-1 min-h-0 w-full overflow-y-auto ">
            <div className="grid grid-cols-2">
                {Array.from({ length: 10 }).map((_, i) => (
                    <div key={i} className="h-40  p-4">
                        <div className="w-full h-full  bg-neutral-700 animate-pulse" />
                    </div>
                ))}
            </div>
        </div>
    );
}