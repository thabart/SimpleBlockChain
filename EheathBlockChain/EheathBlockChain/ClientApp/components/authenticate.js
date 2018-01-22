import React from "react";
import { WebsiteService, SessionService } from '../services';

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
    authenticate(e) {
        e.preventDefault();
        var self = this;
       console.log(SessionService.isExpired());
        /*
        WebsiteService.authenticate(self.state.login, self.state.password).then(function (data) {
            SessionService.setSession(data);
        }).catch(function () {
            console.log("error");
        });*/
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