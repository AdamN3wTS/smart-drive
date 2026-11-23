"use client"
import {useContext,createContext,ReactNode,useState,useEffect, useRef} from "react"
import { useRouter } from "next/navigation";
import { User } from "@/types/user"
import { me, refresh ,logout as logoutLib} from "@/lib/auth";
type AuthContextType = {
    user: User | null,
    token: string | null,
    loading: boolean,
    isLoggedIn: boolean,
    loginContext: (token:string,user:User) =>void,
    logout: ()=>void,
    getValidToken: () => Promise<string | null>,
    refreshUser: () => Promise<void>;
};
const AuthContext = createContext<AuthContextType>({
    user: null,
    token: null,
    loading: true,
    isLoggedIn: false,
    loginContext: () => {},
    logout: () => {},
    getValidToken: async () => null,
    refreshUser: async () => {},
});

export function AuthProvider({children} : {children:ReactNode})
{
    const [user,setUser] = useState<User|null>(null);
    const [token,setToken] = useState<string|null>(null);
    const [loading,setLoading] = useState(true);
        const tokenRef = useRef<string | null>(null);
        const refreshPromiseRef = useRef<Promise<string | null> | null>(null);

    const doRefresh = async (): Promise<string | null> => {
        if (refreshPromiseRef.current) return refreshPromiseRef.current;
        const p = (async () => {
            const refreshResult = await refresh();
            if (refreshResult.success) {
                const newToken = refreshResult.data.token;
                setToken(newToken);
                tokenRef.current = newToken;
                return newToken;
            }
            return null;
        })();
        refreshPromiseRef.current = p;
        try {
            return await p;
        } finally {
            refreshPromiseRef.current = null;
        }
    };

    useEffect(() => {
  const load = async () => {
    try {
      await checkAuth();
    } catch {
      setUser(null);
      setToken(null);
    } finally {
      setLoading(false);
    }
  };
  load();
}, []);
const refreshUser = async () => {
    const newToken = await doRefresh();
    if (newToken) {
        const userMe = await me(newToken);
        if (userMe.success) {
            setUser(userMe.data as User);
        }
    }
};
    const checkAuth = async () =>{
        const usedToken = tokenRef.current;
        if(usedToken){
            const userMe = await me(usedToken);
            if(userMe.success){
                const userData : User = userMe.data as User;
                setUser(userData);
                return;
            }
        }
        const newToken = await doRefresh();
        if (newToken) {
            const userMe = await me(newToken);
            if(userMe.success){
                const userData : User = userMe.data as User;
                setUser(userData);
                return;
            }
        }
        setUser(null);
        setToken(null);
        return;
    }
    const router = useRouter();

const loginContext = (newToken: string, newUser?: User) => {
    if (newToken) {
        setToken(newToken);
        tokenRef.current = newToken;
    }
    if (newUser) setUser(newUser);
    router.push("/");
};

    useEffect(() => {
        tokenRef.current = token;
    }, [token]);

    const getValidToken = async (): Promise<string | null> => {
        if (tokenRef.current) return tokenRef.current;

        const newToken = await doRefresh();
        if (newToken) return newToken;

        return null;
    };
    const logout = async () =>{
        if(token){
            await logoutLib(token);
        }
        setUser(null);
        setToken(null);
        router.push("/");
    }
    const isLoggedIn : boolean = (token !== null && user !== null);
    return (<AuthContext.Provider value={{user,token,loading,isLoggedIn,loginContext,logout,getValidToken ,refreshUser}}>{children}</AuthContext.Provider>)

}
export function UseAuthContext() {
    return useContext(AuthContext);  
}