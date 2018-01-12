import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';

export class Header extends React.Component<{}, {}> {
    public render() {
        return <div className='main-nav'>
            <nav className='navbar navbar-light bg-faded'>
                <a className='navbar-brand' href="#">EHealth BE</a>
                <div className="collapse navbar-collapse">
                    <ul className='navbar-nav mr-auto'>
                        <li className='nav-item active'>
                            <NavLink to={'/'} className='nav-link' href='#'>Home</NavLink>
                        </li>
                    </ul>
                </div>
            </nav>
        </div>;
    }
}
