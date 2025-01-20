export { default } from "next-auth/middleware";

export const config = {
    matcher: [
        // Add your protected routes here
        "/protected/:path*",
        "/api/protected/:path*"
    ]
}; 