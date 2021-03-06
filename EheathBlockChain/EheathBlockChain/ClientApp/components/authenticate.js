﻿import React from "react";
import { WebsiteService, SessionService } from '../services';
import AppDispatcher from '../appDispatcher';
import Constants from '../constants';

class Authenticate extends React.Component {
    constructor(props) {
        super(props);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.authenticate = this.authenticate.bind(this);
        this.state = {
            login: null,
            password: null
        };
    }
    handleInputChange(e) {
        const target = e.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;
        this.setState({
            [name]: value
        });
    }
    /* Authenticate the user */
    authenticate(e) {
        e.preventDefault();
        var self = this;
        WebsiteService.authenticate(self.state.login, self.state.password).then(function (data) {
            SessionService.setSession(data);
            AppDispatcher.dispatch({
                actionName: Constants.events.USER_LOGGED_IN
            });
        }).catch(function (e) {
            console.log("error");
        });
    }
    render() {
        return (<div>
            <h1>S'authentifier</h1>
            <form onSubmit={this.authenticate}>
                <div className="form-group">
                    <label>Login</label>
                    <input type="text" className="form-control" name="login" onChange={this.handleInputChange} />
                </div>
                <div className="form-group">
                    <label>Mot de passe</label>
                    <input type="password" className="form-control" name="password" onChange={this.handleInputChange} />
                </div>
                <button className="btn btn-primary">Se connecter</button>
            </form>
        </div>);
    }
}

export default Authenticate;