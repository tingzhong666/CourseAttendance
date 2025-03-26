import React from 'react';
import { Button, Result } from 'antd';
import { useNavigate } from 'react-router';

const NotFount = () => {
    const navigate = useNavigate();

    const backHome = (x: React.MouseEvent<HTMLElement, MouseEvent>): void => {
        navigate("/");
    }

    return (
        <Result
            status="404"
            title="404"
            subTitle="Sorry, the page you visited does not exist."
            extra={<Button type="primary" onClick={x => backHome(x)}>Back Home</Button>}
        />
    )
}

export default NotFount