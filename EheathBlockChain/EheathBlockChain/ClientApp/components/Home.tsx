import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>Ajouter une prescription médicale</h1>
            <form>
                <div className="form-group">
                    <label></label>
                </div>
                <div className="form-group">

                </div>
                <div className="form-group">

                </div>
            </form>
        </div>;
    }
}
