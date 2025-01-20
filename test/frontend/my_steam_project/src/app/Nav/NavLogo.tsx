'use client'

import React from 'react'
import { GrGamepad } from 'react-icons/gr'
import { usePathname,useRouter } from 'next/navigation'

export default function NavLogo() {
    const router = useRouter();
    const pathName = usePathname();

    const goToHomePage = () => {
            router.push('/');
    }


  return (
    <div  className='cursor-pointer' onClick={goToHomePage} >
    <div className='col'>
    <GrGamepad size={50} /> 
    </div>
</div>
  )
}
