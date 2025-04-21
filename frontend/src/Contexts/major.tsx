import { createContext, ReactNode, useContext, useEffect, useState } from "react"
import { GradeResponseDto, MajorsCategoryResDto, MajorsSubcategoryResDto } from "../services/api"
import * as api from '../services/http/httpInstance'

export interface MajorsSubcategoryData extends MajorsSubcategoryResDto {
    // 父专业名
    parentName: string
}

interface ClassesStr {
    name: string
    id: number
}

interface MajorType {
    majorsCategories: Array<MajorsCategoryResDto>
    majorsSubcategories: Array<MajorsSubcategoryData>
    classes: Array<GradeResponseDto>
    classesStr: Array<ClassesStr>
    update: () => void
}
const MajorContext = createContext<MajorType>({} as MajorType);

interface Props {
    children: ReactNode
}
export const MajorProvider = ({ children }: Props) => {
    const [majorsCategories, setMajorsCategories] = useState<Array<MajorsCategoryResDto>>([])
    const [majorsSubcategories, setMajorsSubcategories] = useState<Array<MajorsSubcategoryData>>([])
    const [classes, setClasses] = useState<Array<GradeResponseDto>>([])
    const [classesStr, setClassesStr] = useState<Array<ClassesStr>>([])


    useEffect(() => {
        init()
    }, [])

    const init = async () => {
        const majorsCategories_ = await getMajorsCategories()
        const majorsSubcategories_ = await getMajorsSubcategories(majorsCategories_)
        const classes_ = await getClasses()

        const tmp: ClassesStr[] = classes_.map(x => {
            const majorsSubcategory = majorsSubcategories_.find(v => v.id == x.majorsSubcategoriesId)
            const majorsCategory = majorsCategories_.find(v => v.id == majorsSubcategory?.majorsCategoriesId)

            return {
                name: `${x.year}级-${majorsCategory?.name}-${majorsSubcategory?.name}-${x.num}班`,
                id: x.id || -1
            }
        })

        setClassesStr(tmp)

    }
    const getClasses = async () => {
        const res = await api.Classes.apiClassesGet()
        setClasses(res.data.data?.dataList ?? [])
        return res.data.data?.dataList ?? []

    }
    const getMajorsSubcategories = async (majorsCategories_: MajorsCategoryResDto[]) => {
        const res = await api.MajorsSubcategory.apiMajorsSubcategoryGet(undefined, 1, 9999)
        const tmp2 = res.data.data?.dataList?.map(x => {
            const majorsCategory = majorsCategories_.find(v => v.id == x.majorsCategoriesId)

            const tmp = x as MajorsSubcategoryData

            tmp.parentName = majorsCategory?.name || ''
            return tmp
        })

        setMajorsSubcategories(tmp2 ?? [])
        return tmp2 ?? []

    }
    const getMajorsCategories = async () => {
        const res = await api.MajorsCategory.apiMajorsCategoryGet(1, 9999)
        setMajorsCategories(res.data.data?.dataList ?? [])

        return res.data.data?.dataList ?? []
    }

    return (<MajorContext.Provider value={{ majorsCategories, majorsSubcategories, classes, classesStr, update: init }}>
        {children}
    </MajorContext.Provider>)
}

export const useMajor = () => useContext(MajorContext)