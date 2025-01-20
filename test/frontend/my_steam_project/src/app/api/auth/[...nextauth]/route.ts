import { NextAuthOptions } from "next-auth";
import NextAuth from "next-auth/next";
import DuendeIdentityServer6 from "next-auth/providers/duende-identity-server6";

export const authenticationSettings: NextAuthOptions = {
    secret: process.env.NEXTAUTH_SECRET,
    session: {
        strategy: 'jwt',
        maxAge: 30 * 24 * 60 * 60, // 30 days
    },
    providers: [
        DuendeIdentityServer6({
            id: 'id-server',
            clientId: 'frontend',
            clientSecret: 'BigSecret',
            issuer: 'http://localhost:5001',
            authorization: {
                params: {
                    scope: 'openid profile microserviceApp'
                },
                timeout: 10000
            },
            idToken: true,
            httpOptions: {
                timeout: 10000
            }
        })
    ],
    debug: true,
    callbacks: {
        async jwt({ token, profile, account }) {
            if (profile) {
                token.username = profile.username;
                token.role = profile.role;
            }
            if (account) {
                token.access_token = account.access_token;
            }
            return token;
        },
        async session({ session, token }) {
            if (token) {
                session.user.username = token.username;
                session.user.role = token.role;
            }
            return session;
        },
    },
    jwt: {
        maxAge: 30 * 24 * 60 * 60, // 30 days
    },
    cookies: {
        sessionToken: {
            name: `next-auth.session-token`,
            options: {
                httpOnly: true,
                sameSite: 'lax',
                path: '/',
                secure: process.env.NODE_ENV === 'production'
            }
        }
    }
};

const handler = NextAuth(authenticationSettings);
export { handler as GET, handler as POST, handler as PUT };