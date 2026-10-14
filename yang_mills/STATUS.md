# STATUS

Current task: adversarial attack on YM-191-COMPLEX-MAJORANT-1, starting at V=0. The reduced analysis is complete to the failure/candidate results below; it does not close the global reduced map.

STATUS:
OLD UNRESTRICTED INDEPENDENT CHART REFUTED AS AN UNQUALIFIED THEOREM.
SOURCE-COMPATIBLE REDUCED C^1: GAP.
STRICT WHOLE-BALL IMAGE INCLUSION: NOT PROVED.
GLOBAL UNIFORMITY OVER Sigma: UNKNOWN.

V=0 necessary test:
mu_F=(1+beta)kappa-lambda_in.
For the precise indexed T-domain family P_T, the old mixed envelope yields the sharp test
C_Tgeo(lambda_in-beta kappa)<infinity,
provided its old anchored envelope normalizer is finite.
The old theorem did not assume this.

Counterexample:
On all connected polymers, take nested cube chains X_n containing B_0 and local evaluation operators E_n with ||E_n||=1,E_n1=1.
T_{X_n}=c A_T e^{-kappa d(X_n)}E_n, T_X=0 otherwise, V=0.
A fixed c>0 makes ||T||_old<=1/2, while
sum_n e^{mu_F d(X_n)}|T_{X_n}1|
=c A_T sum_n e^{(mu_F-kappa)d(X_n)}=infinity
for mu_F>=kappa. Finite-volume truncations have uniformly bounded inputs and unbounded outputs. Shrinking any positive scalar radius does not fix the exponent/entropy mismatch.

Qualification:
The test passes if lambda_in-beta kappa lies in a verified geometric-summability regime; it also passes directly if mu_F<=mu_T. A source-restricted family with additional size/coupling rules needs its own uniform certificate. The unrestricted counterexample is NOT claimed to be a realizable Balaban history or a criticism of the physical theory.

T operator:
conditional integration/history kernels on the full hidden-variable feasible fiber -> retained external configurations; use the stable conditional TV/operator norm.
Real positivity makes mass T^0_X1 control the real operator. The source complex comparison/normalized bound controls its admitted continuation. A scalar T_X1 bound alone never controls an arbitrary complex operator.
The exact inherited-variable/extension dictionary remains unextracted.

New charts:
A. Independent stable-operator/TV weights at nu_T=mu_F+lambda_T, C_geo(lambda_T)<infinity; likewise a stronger ordinary V weight. V=0 is controlled. Physical source embedding and the full majorant are UNKNOWN.
B. Relative density parameters b_X in B_b(full history/integration fiber), with kernels (e^{sigma_X}+b_X)nu_X^0. For |sigma_X|<=1, a fixed epsilon=e^{-2}/2 gives an open complex parameter ball and uniform local normalization stability. Physical operators are its centers. This is not an open set of physical histories and does not create missing spatial decay.

Full majorant:
A labelled positive iterated-kernel series is written in rprime_191_vzero_attack.md, including all restrictions, history/orientation weights and operator extensions. Its native uniform anchored bound at EXACTLY mu_F is NOT PROVED.

Concrete remaining reduced subsum:
q=1,D={Y}, V-domain attached to a T-component. The V bound is relative to Z_loc(h); avoiding the second-class Ycal does not remove first-class T cores from that relative geometry. Size/energy payment and the attached-Y entropy have not been certified on the new complex chart.

Exact new blockers:
YM-191-T-STABLE-DOMAIN-1;
YM-191-RELATIVE-GEOMETRY-1;
then YM-191-COMPLEX-MAJORANT-1 and YM-UPSTREAM-IMAGE-RESERVE-1.

Exact next step:
instantiate the native T support/size/coupling bound and the conditional embedding inequality
sup_{g_k<=gamma} exp[s_0-c_0 p_0(g_k)+nu_T D_k]<infinity/sufficiently small,
then bound the q=1,D={Y} attached-domain sum in that SAME hidden-variable chart.

V-producer: DEFERRED; not attacked in this continuation because the reduced map is still open.

Accepted unchanged:
F -> native regular R'_Bal bounded C^1;
C_inc=9, c_stitch=sqrt(7);
r_*=(a/9)exp[-16a-sqrt(7)theta].
The physical source F norm certificate C_geo(lambda_in)e^{-p_0(gamma)} remains valid under its accepted assumptions. It is not a whole complex-ball inclusion. The stronger downstream Schur result remains conditional on its explicit reserve.

Detailed proofs, counterexample and status table: rprime_191_vzero_attack.md.
Prior audit/radius discipline: rprime_upstream_c1.md.
No K_0, STEP_0, boundary B', physical source derivatives or other RG sectors.

The unrelated historical crossover records below were not worked on.

## Update — Iteration 2 (YM-CROSSOVER-1 only)

STATUS:
YM-CROSSOVER-1 REDUCED TO EXPLICIT SUBLEMMAS; inclusion itself NOT proved.

Strongest new result:
The measure-only claim
R^M(K_match) subset D_KP
is insufficient for the original microscopic mass gap unless source/observable transport is controlled. The corrected crossover package is:

(A) YM-MATCH-EXTRACT-1:
extract a global, cutoff/volume-uniform K_match from the UV construction in the concrete Banach space of crossover_spec.md.

(B) YM-TUBE-1 + YM-RG-DRIFT-1:
prove finite one-step enclosures for beta and the full remainder norm. Once a uniform drift delta>0 and invariant remainder radius R exist, M_G<infinity follows and is no longer an independent heuristic assumption.

(C) YM-RG-SOURCE-1:
control source transport / conditional covariance across eliminated RG shells.

(D) Final KP landing:
Q_{alpha,mu}(z_Phi)<alpha. YM-KP-ANCHOR-1 then gives uniform cluster expansion and exponential coarse correlations.

Concrete Banach space:
B_{alpha,mu,p,rho} of globally defined gauge-invariant polymer interactions Phi_X on connected 4D block polymers, with norm
sup_B sum_{X contains B} exp(alpha|X|+mu diam(X)) ||Phi_X||_{C^p,rho}.
Full definition: crossover_spec.md.

Corrected blocker:
The hardest unresolved estimate is no longer the abstract statement "M_G finite". It is the construction of rigorous one-step Yang-Mills enclosures

F_j^-(beta,r) <= beta(R Phi) <= F_j^+(beta,r),
r(R Phi) <= G_j(beta,r)

on an invariant tube, together with source-locality estimates, starting from a rigorously extracted K_match.

Verified this iteration:
- scalar-coupling-only flow is insufficient (explicit toy counterexamples);
- q<alpha in the anchored weighted polymer norm is a sufficient KP condition;
- the finite invariant-tube induction is rigorous;
- a positive uniform beta drift plus invariant remainder tube implies cutoff-independent finite M;
- previous cutoff bridge can be strengthened to positive-time correlator convergence only (YM-CUTOFF-BRIDGE-2), so divergent equal-time renormalization constants/prefactors need not be assumed controlled.

Validated-numerics support:
scripts/verify_crossover_tube.py uses exact rational interval arithmetic to certify the finite tube induction once rigorous one-step bounds are supplied. The bundled JSON is explicitly only a toy example.


### Final refinement — Iteration 2

The one-step invariant-tube problem has now been reduced further by YM-RG-DERIV-1 and YM-RG-TAYLOR-1.

For Phi=beta W+eta with ell(eta)=0 and ||eta||<=r, define
f(beta)=ell(R(beta W)),
h(beta)=||Q R(beta W)||.
A rigorous one-step tube enclosure follows from five scalar bound families on each compact beta interval I_j:

1. center flow: f_j^- <= f(beta) <= f_j^+;
2. generated center remainder: h(beta) <= H_j;
3. relevant/irrelevant mixing: ||ell D R_{beta W} Q|| <= A_j;
4. irrelevant amplification: ||Q D R_{beta W} Q|| <= B_j;
5. curvature: sup_{Phi in K_j} ||D^2 R_Phi|| <= C_j.

It then suffices that

beta_{j+1}^- <= f_j^- - A_j R_j - (1/2)||ell|| C_j R_j^2,

beta_{j+1}^+ >= f_j^+ + A_j R_j + (1/2)||ell|| C_j R_j^2,

R_{j+1} >= H_j + B_j R_j + (1/2)||Q|| C_j R_j^2.

For the exact block-integration part of R,
D E_Phi[A]=E_{Phi,V}[A] and
D^2 E_Phi[A,B]=-Cov_{Phi,V}(A,B).
Thus the fifth bound is concretely a one-shell conditional-covariance estimate, not an abstract Frechet-regularity placeholder.

Current deepest blocker:
obtain rigorous, volume-independent Yang-Mills bounds for f,H,A,B,C on at least the first intermediate-coupling interval emerging from YM-MATCH-EXTRACT-1. This is the exact next step.


### Adversarial corrections — final

1. D_KP is NOT equivalent to a mass gap. It is only a sufficient landing chart; even exactly product measures can violate a badly chosen product-Haar activity smallness condition if local weights are large. Therefore failure of YM-CROSSOVER-1 in this chart would not imply gaplessness.

2. Polymer incompatibility must be defined using the actual collar/link support of Phi_X, not just the core block set.

3. Source control must start before the finite crossover: the UV-to-matching package must deliver quasi-local source kernels for microscopic gauge-invariant observables (YM-UV-SOURCE-MATCH-1).

4. The practical finite-drift lemma is YM-RG-DRIFT-2, with a bounded terminal beta strip and a no-overshoot estimate.

Final status of Iteration 2:
STATUS: YM-CROSSOVER-1 REDUCED TO EXPLICIT LEMMAS/INEQUALITIES — NOT PROVED.

For the measure inclusion itself, the remaining Yang-Mills content is:
- MATCH: rigorous K_match extraction in the concrete global norm;
- STEP_j: rigorous f_j^±, H_j, A_j, B_j, C_j bounds for finitely many intermediate-coupling intervals;
- LAND: final Q_{alpha,mu}<alpha estimate.

For use in the mass-gap chain, additionally:
- UV-SOURCE: microscopic-to-matching source localization;
- CROSSOVER-SOURCE: shell-by-shell source/conditional-covariance bounds.

Exact next attack:
prove or falsify the FIRST one-step certificate STEP_0 in the chosen norm, starting from the strongest matching estimates that can actually be extracted from Balaban's small-field/large-field RG results.
