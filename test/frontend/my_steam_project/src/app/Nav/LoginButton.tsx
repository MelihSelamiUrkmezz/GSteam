'use client'
import { Button } from 'flowbite-react'
import { signIn } from 'next-auth/react'
import React from 'react'
import { toast } from 'react-hot-toast'

export default function LoginButton() {
  const handleSignIn = async () => {
    try {
      await signIn('id-server', {
        callbackUrl: '/',
        redirect: true
      })
    } catch (error) {
      console.error('Sign in error:', error)
      toast.error('Failed to sign in. Please try again.')
    }
  }

  return (
    <Button outline onClick={handleSignIn}>
      Login
    </Button>
  )
}
